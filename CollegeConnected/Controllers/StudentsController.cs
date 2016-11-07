using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class StudentsController : Controller
    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();

        // GET: Students
        public ActionResult Index()
        {
            var studentList = (from student in db.Students
                select student).ToList();
            return View(studentList);
        }

        // GET: Students/Details/5
        public ActionResult Details()
        {
            var studentList = (from student in db.Students
                select student).ToList();
            return View(studentList);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,LastName,Address1,Address2,ZipCode,State,PhoneNumber,Email,GraduationYear,BirthDate,UpdateTimeStamp"
             )] Student student)
        {
            if (ModelState.IsValid)
            {
                student.StudentId = Guid.NewGuid();
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var student = db.Students.Find(id);
            if (student == null)
                return HttpNotFound();
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,LastName,Address1,Address2,ZipCode,State,PhoneNumber,Email,GraduationYear,BirthDate,UpdateTimeStamp"
             )] Student student)
        {
            if (ModelState.IsValid)
            {
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
            var student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        public void ExportToCsv()
        {
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Student Number\",\"First Name\",\"Middle Name\",\"Last Name\",\"Address1\"," +
                         "\"Address2\",\"Zip Code\",\"City\",\"State\",\"Phone Number\",\"Email\",\"Graduation Year" +
                                           "\"Birthday\"");
                Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=ExportedStudents_" + DateTime.Now + ".csv");
            Response.ContentType = "text/csv";

            var students = db.Students.ToList();

            foreach (var student in students)
            {
                sw.WriteLine(
                    $"\"{student.StudentNumber}\",\"{student.FirstName}\",\"{student.MiddleName}\",\"{student.LastName}\",\"{student.Address1}\"," +
                    $"\"{student.Address2}\",\"{student.ZipCode}\",\"{student.City}\",\"{student.State}\",\"{student.PhoneNumber}\",\"{student.Email}\"," +
                    $"\"{student.GraduationYear}\",\"{student.BirthDate}\"");
            }

            Response.Write(sw.ToString());
            Response.End();
        }

    }
}