using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using CollegeConnected.DataLayer;
using CollegeConnected.Models;
using CollegeConnected.Services;

namespace CollegeConnected.Controllers
{
    public class ConstituentsController : Controller

    {
        private SharedControllerOperations sharedOperations = new SharedControllerOperations();
        private readonly UnitOfWork db = new UnitOfWork();

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

        public void ExportToCsv()
        {
            var sw = new StringWriter();

            sw.WriteLine("\"Student Number\",\"First Name\",\"Middle Name\",\"Last Name\",\"Address1\"," +
                         "\"Address2\",\"Zip Code\",\"City\",\"State\",\"Phone Number\",\"Email\",\"Graduation Year" +
                         "\"Birthday\"");
            Response.ClearContent();
            Response.AddHeader("content-disposition",
                "attachment;filename=ExportedConstituents_" + DateTime.Now + ".csv");
            Response.ContentType = "text/csv";

            var students = db.StudentRepository.Get();

            foreach (var student in students)
                sw.WriteLine(
                    $"\"{student.StudentNumber}\",\"{student.FirstName}\",\"{student.MiddleName}\",\"{student.LastName}\",\"{student.Address1}\"," +
                    $"\"{student.Address2}\",\"{student.ZipCode}\",\"{student.City}\",\"{student.State}\",\"{student.PhoneNumber}\",\"{student.Email}\"," +
                    $"\"{student.FirstGraduationYear}\",\"{student.BirthDate}\"");

            Response.Write(sw.ToString());
            Response.End();
        }
    }
}