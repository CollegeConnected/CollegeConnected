using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using CollegeConnected.DataLayer;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class ConstituentsController : Controller

    {
        private readonly UnitOfWork db = new UnitOfWork();
        private readonly BaseController sharedOperations = new BaseController();

        public ActionResult Index()
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                var studentList = db.StudentRepository.Get();
                return View(studentList);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Create()
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
                return View();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
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

        public ActionResult Edit(Guid? id)
        {
            ViewBag.Years = sharedOperations.GenerateGradYearList();
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var student = db.StudentRepository.GetById(id);
                if (student == null)
                    return HttpNotFound();
                return View(student);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
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

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var student = db.StudentRepository.GetById(id);
            if (student == null)
                return HttpNotFound();
            return View(student);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                var student = db.StudentRepository.GetById(id);
                db.StudentRepository.Delete(student);
                db.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}