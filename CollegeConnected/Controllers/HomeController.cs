using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using CollegeConnected.DataLayer;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class HomeController : Controller
    {
        private readonly SharedControllerOperations sharedOperations = new SharedControllerOperations();
        private readonly UnitOfWork db = new UnitOfWork();

        public ActionResult Admin()
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                var today = DateTime.Now;
                var yearAgo = today.AddDays(-365);

                var count = db.StudentRepository.Get().Count();
                var sinceCount =
                    db.StudentRepository.Get(
                        student => student.UpdateTimeStamp >= yearAgo && student.UpdateTimeStamp <= today).Count();
                ViewBag.Count = count;
                ViewBag.sinceCount = sinceCount;

                ViewBag.Message = "Your Admin Home Page.";

                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            return View(db.StudentRepository.Get(student => student.StudentNumber == "").ToList());
        }

        [HttpPost]
        public ActionResult Index(string studentNumber, string studentLastName)
        {
            if (string.IsNullOrEmpty(studentLastName))
            {
                var studentList = db.StudentRepository.Get(student => student.StudentNumber == studentNumber).ToList();
                if (!studentList.Any())
                {
                    ModelState.AddModelError("Error", "No results found. Click the Register button to sign up for collegeConnected.");
                }
                return View(studentList);
            }
            if (string.IsNullOrEmpty(studentNumber))
            {
                var studentList = db.StudentRepository.Get(student => student.LastName == studentLastName).ToList();
                if (!studentList.Any())
                {
                    ModelState.AddModelError("Error", "No results found. Click the Register button to sign up for collegeConnected.");
                }
                return View(studentList);
            }
            else
            {
                var studentList = db.StudentRepository.Get(student => student.StudentNumber == studentNumber).ToList();
                if (!studentList.Any())
                {
                    ModelState.AddModelError("Error", "No results found. Click the Register button to sign up for collegeConnected.");
                }
                return View(studentList);
            }
        }
        
        public ActionResult Confirm(Guid? id)
        {
            ViewBag.Years = sharedOperations.GenerateGradYearList();
            if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var student = db.StudentRepository.GetById(id);
            if (student == null)
                    return HttpNotFound();
            return View(student);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(
            [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber," +
                     "Email,FirstGraduationYear,SecondGraduationYear,ThirdGraduationYear,BirthDate,UpdateTimeStamp,ConstituentType," +
                     "AllowCommunication,HasAttendedEvent,EventsAttended"
             )] Constituent student)
        {
            ViewBag.Years = sharedOperations.GenerateGradYearList();
            if (ModelState.IsValid)
            {
                student.UpdateTimeStamp = DateTime.Now;
                db.StudentRepository.Update(student);
                db.Save();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        public ActionResult Verify(Guid? id)
        {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var student = db.StudentRepository.GetById(id);
            return View(student);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Verify(Guid? id, DateTime birthDate)
        {
            var student = db.StudentRepository.GetById(id);
            var bday = student.BirthDate;

            if (bday == birthDate)
                return RedirectToAction("Confirm", new { id });
            ModelState.AddModelError("", "Birthday incorrect");
            return View();
        }

        public ActionResult Register()
        {
            ViewBag.Years = sharedOperations.GenerateGradYearList();
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(
        [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber," +
                     "Email,FirstGraduationYear,SecondGraduationYear,ThirdGraduationYear,BirthDate,UpdateTimeStamp,ConstituentType," +
                     "AllowCommunication,HasAttendedEvent,EventsAttended"
             )] Constituent student)
        {
             ViewBag.Years = sharedOperations.GenerateGradYearList();
            var rowExists = db.StudentRepository.dbSet.Any(s => s.StudentNumber.Equals(student.StudentNumber));
            try
            {
                if (ModelState.IsValid)
                {
                    if (student.StudentNumber == null)
                    {
                        student.StudentId = Guid.NewGuid();
                        student.UpdateTimeStamp = DateTime.Now;
                        student.HasAttendedEvent = false;
                        student.EventsAttended = 0;
                        db.StudentRepository.Insert(student);
                        db.Save();
                        return RedirectToAction("Index");
                    }
                    if (!rowExists)
                    {
                        student.StudentId = Guid.NewGuid();
                        student.UpdateTimeStamp = DateTime.Now;
                        student.HasAttendedEvent = false;
                        student.EventsAttended = 0;
                        db.StudentRepository.Insert(student);
                        db.Save();
                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError("Error",
                        "This student number already exists in the system. Search for the person from the Home page.");
                    return View(student);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error",
                    "An error occurred saving this constituent. If the problem persits, contact your system administrator." +
                    $"Exception: {e}");
            }
            return View(student);
        }

        public ActionResult PurgeData()
        {
            db.EventAttendanceRepository.db.Database.ExecuteSqlCommand("DELETE FROM EventAttendances");
            db.StudentRepository.db.Database.ExecuteSqlCommand("DELETE FROM Students");
            db.EventRepository.db.Database.ExecuteSqlCommand("DELETE FROM Events");
            return RedirectToAction("Admin");
        }
    }
}