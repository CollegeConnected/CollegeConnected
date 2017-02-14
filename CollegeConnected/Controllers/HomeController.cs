using System;
using System.Linq;
using System.Web.Mvc;
using CollegeConnected.Models;
using System.Net;
using System.Data.Entity;

namespace CollegeConnected.Controllers
{
    public class HomeController : Controller
    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();

        public ActionResult Admin()
        {
            int count = db.Students.Count();
            DateTime today = DateTime.Now;
            DateTime yearAgo = today.AddDays(-365);
            int sinceCount =
            (from row in db.Students
                where row.UpdateTimeStamp >= yearAgo && row.UpdateTimeStamp <= today
                select row).Count();
            ViewBag.Count = count;
            ViewBag.sinceCount = sinceCount;

            ViewBag.Message = "Your Admin Home Page.";

            return View();
        }

        public ActionResult Index()
        {
            string searchString = "";
            var studentList = (from student in db.Students
                               where student.StudentNumber == searchString
                               select student
                   ).ToList();
            return View(studentList);
        }

        [HttpPost]
        public ActionResult Index(string studentNumber, string studentLastName)
        {
            if (string.IsNullOrEmpty(studentLastName))
            {
                var studentList = (from student in db.Students
                                   where student.StudentNumber == studentNumber
                                   select student
                                   ).ToList();
                return View(studentList);
            }
            else if (string.IsNullOrEmpty(studentNumber))
            {
                var studentList = (from student in db.Students
                                   where student.LastName == studentLastName
                                   select student
                                   ).ToList();
                return View(studentList);
            }
            else
            {
                var studentList = (from student in db.Students
                                   where student.StudentNumber == studentNumber
                                   select student
                                   ).ToList();
                return View(studentList);
            }
        }

        // GET: Students/Edit/5
        public ActionResult Confirm(Guid? id)
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
        public ActionResult Confirm(
            [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber,Email,GraduationYear,BirthDate,UpdateTimeStamp,ConstituentType,AllowCommunication"
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
        /*  public ActionResult Report()
        {
            ReportViewer rptViewer = new ReportViewer();

            // ProcessingMode will be Either Remote or Local  
            rptViewer.ProcessingMode = ProcessingMode.Remote;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            rptViewer.AsyncRendering = true;
            rptViewer.ServerReport.ReportServerUrl = new Uri("http://localhost/ReportServer/");

            rptViewer.ServerReport.ReportPath

            ViewBag.ReportViewer = rptViewer;
            return View();
        }*/
    }
}