using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CollegeConnected.Models;
using System.Web.Security;

namespace CollegeConnected.Controllers
{
    public class StudentsController : Controller

    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();

        public ActionResult Register()
        {
            if (isAuthenticated())
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Students
        public ActionResult Index()
        {
            if (isAuthenticated())
            {
                var studentList = (from student in db.Students
                                   select student).ToList();
                return View(studentList);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Students/Details/5
        public ActionResult Details()
        {
            if (isAuthenticated())
            {
                var studentList = (from student in db.Students
                                   select student).ToList();
                return View(studentList);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            if (isAuthenticated())
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber,Email,FirstGraduationYear,SecondGraduationYear,ThirdGraduationYear,BirthDate,UpdateTimeStamp,ConstituentType,AllowCommunication,HasAttendedEvent,EventsAttended"
             )] Student student)
        {
            if (student.StudentNumber == null)
            {
                student.StudentId = Guid.NewGuid();
                student.UpdateTimeStamp = DateTime.Now;
                student.HasAttendedEvent = false;
                student.EventsAttended = 0;
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                
                    bool rowExists = db.Students.Any(s => s.StudentNumber.Equals(student.StudentNumber));

                    if (ModelState.IsValid && !rowExists)
                    {
                        student.StudentId = Guid.NewGuid();
                    student.UpdateTimeStamp = DateTime.Now;
                    student.HasAttendedEvent = false;
                        student.EventsAttended = 0;
                        db.Students.Add(student);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "This student number already exists in the system. Search for the person from the Home page.");

                    }
                }
                return View(student);
            }
        // GET: Students/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (isAuthenticated())
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var student = db.Students.Find(id);
                if (student == null)
                    return HttpNotFound();
                return View(student);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber,Email,FirstGraduationYear,SecondGraduationYear,ThirdGraduationYear,BirthDate,UpdateTimeStamp,ConstituentType,AllowCommunication,HasAttendedEvent,EventsAttended"
             )] Student student)
        {
            if (ModelState.IsValid)
            {
                student.UpdateTimeStamp = DateTime.Now;
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var student = db.Students.Find(id);
            if (student == null)
                return HttpNotFound();
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            if (isAuthenticated())
            {
                var student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        public void ExportToCsv()
        {
            var sw = new StringWriter();

            sw.WriteLine("\"Student Number\",\"First Name\",\"Middle Name\",\"Last Name\",\"Address1\"," +
                         "\"Address2\",\"Zip Code\",\"City\",\"State\",\"Phone Number\",\"Email\",\"Graduation Year" +
                         "\"Birthday\"");
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=ExportedConstituents_" + DateTime.Now + ".csv");
            Response.ContentType = "text/csv";

            var students = db.Students.ToList();

            foreach (var student in students)
                sw.WriteLine(
                    $"\"{student.StudentNumber}\",\"{student.FirstName}\",\"{student.MiddleName}\",\"{student.LastName}\",\"{student.Address1}\"," +
                    $"\"{student.Address2}\",\"{student.ZipCode}\",\"{student.City}\",\"{student.State}\",\"{student.PhoneNumber}\",\"{student.Email}\"," +
                    $"\"{student.FirstGraduationYear}\",\"{student.BirthDate}\"");

            Response.Write(sw.ToString());
            Response.End();
        }
        private bool isAuthenticated()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                if (ticket != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}