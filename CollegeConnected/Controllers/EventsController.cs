using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using CollegeConnected.Models;
using System.IO;

namespace CollegeConnected.Controllers
{
    public class EventsController : Controller
    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();

        public ActionResult Index()
        {
            if (isAuthenticated())
                return View(db.Events.ToList());
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(Guid? id)
        {
            if (isAuthenticated())
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var @event = db.Events.Find(id);
                if (@event == null)
                    return HttpNotFound();
                return View(@event);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Create()
        {
            if (isAuthenticated())
                return View();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "EventID,EventName,EventLocation,EventStartDateTime,EventEndDateTime,Attendance")] Event e)
        {
            if (ModelState.IsValid)
            {
                e.EventID = Guid.NewGuid();
                e.Attendance = 0;
                e.EventStatus = "In Progress";
                e.CreatedBy = "Administrator ";
                db.Events.Add(e);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(e);
        }

        public ActionResult Edit(Guid? id)
        {
            if (isAuthenticated())
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var @event = db.Events.Find(id);
                if (@event == null)
                    return HttpNotFound();
                return View(@event);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "EventID,EventName,EventStatus,CreatedBy,EventLocation,EventStartDateTime,EventEndDateTime,Attendance")
            ] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult CompleteEvent(Guid id)
        {
            var @event = db.Events.Find(id);
            @event.EventStatus = "Completed";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ReactivateEvent(Guid id)
        {
            var @event = db.Events.Find(id);
            @event.EventStatus = "In Progress";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SignIn(Guid id, string message)
        {
            ViewBag.Message = message;
            if (isAuthenticated())
            {
                var ccEvent = db.Events.Find(id);
                var eventTitle = ccEvent.EventName;
                ViewBag.Title = eventTitle;
                var searchString = "";
                var studentList = (from student in db.Students
                    where student.StudentNumber == searchString
                    select student
                ).ToList();
                return View(studentList);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SignIn(string studentNumber, string studentLastName, Guid id)
        {
            var ccEvent = db.Events.Find(id);
            var eventTitle = ccEvent.EventName;
            ViewBag.Title = eventTitle;
            if (string.IsNullOrEmpty(studentLastName))
            {
                var studentList = (from student in db.Students
                    where student.StudentNumber == studentNumber
                    select student
                ).ToList();
                if (studentList.Count() == 0)
                {
                    ModelState.AddModelError("Error", "No results found. Click the Register button to sign up for collegeConnected.");
                }
                return View(studentList);
            }
            if (string.IsNullOrEmpty(studentNumber))
            {
                var studentList = (from student in db.Students
                    where student.LastName == studentLastName
                    select student
                ).ToList();
                if (studentList.Count() == 0)
                {
                    ModelState.AddModelError("Error", "No results found. Click the Register button to sign up for collegeConnected.");
                }
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

        public ActionResult Confirm(Guid? id, Guid? eventId)
        {
            if (isAuthenticated())
            {
                var student = db.Students.Single(x => x.StudentId == id);
                var ccEvent = db.Events.Single(x => x.EventID == eventId);
                var eventViewModel = new EventViewModel(student, ccEvent);
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                if (eventViewModel == null)
                    return HttpNotFound();
                
                return View(eventViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(
            [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber,Email,FirstGraduationYear,SecondGraduationYear,ThirdGraduationYear,BirthDate,UpdateTimeStamp,ConstituentType,AllowCommunication,HasAttendedEvent,EventsAttended"
             )] Student student,
            Guid id)
        {
            string thankYou = "Thank you for signing in.";

            var ccEvent = db.Events.Find(id);
            if (ModelState.IsValid)
            {
                var a = AttendEvent(student.StudentId, id);
                if (a == -1)
                {
                    ModelState.AddModelError("Error", "You have already signed into this event.");
                    // ViewBag.Error = "You have already signed into this event";
                }
                else
                {
                    if (!student.HasAttendedEvent)
                        student.HasAttendedEvent = true;
                    student.EventsAttended++;
                    ccEvent.Attendance++;
                    student.UpdateTimeStamp = DateTime.Now;
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("SignIn", new {id = id, message = thankYou});
                }
            }
            return View();
        }

        public int AttendEvent(Guid studentId, Guid eventId)
        {
            var rowExists = db.EventAttendants.Any(ev => ev.StudentId.Equals(studentId) && ev.EventId.Equals(eventId));

            if (rowExists)
            {
                return -1;
            }
            var eventAttendant = new EventAttendance(Guid.NewGuid(), studentId, eventId);
            db.EventAttendants.Add(eventAttendant);
            db.SaveChanges();
            return 0;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Verify(Guid studentId, Guid eventId, DateTime BirthDate)
        {
            var student = db.Students.Find(studentId);
            var bday = student.BirthDate;
            var evID = eventId;
            ViewBag.EventID = eventId;
            if (bday == BirthDate)
                return RedirectToAction("Confirm", "Events", new {id = studentId, eventId});
            ModelState.AddModelError("", "Birthday incorrect");
            var ccStudent = db.Students.Single(x => x.StudentId == studentId);
            var ccEvent = db.Events.Single(x => x.EventID == eventId);
            var eventViewModel = new EventViewModel(student, ccEvent);
            return View(eventViewModel);
        }

        public ActionResult Verify(Guid? id, Guid eventId)
        {
            if (isAuthenticated())
            {
                ViewBag.EventID = eventId;
                var student = db.Students.Single(x => x.StudentId == id);
                var ccEvent = db.Events.Single(x => x.EventID == eventId);
                var eventViewModel = new EventViewModel(student, ccEvent);
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                if (eventViewModel == null)
                    return HttpNotFound();
                return View(eventViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult VerifyCompleteEvent(Guid id)
        {
            if (isAuthenticated())
            {
                var ccEvent = db.Events.Single(x => x.EventID == id);
                var user = db.Users.Find("admin@unf.edu");
                var password = user.Password;
                var viewModel = new CompleteEventViewModel(ccEvent, password);
                return View(viewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult VerifyCompleteEvent(Guid id, string password)
        {
            var user = db.Users.Find("admin@unf.edu");
            var bytes = Encoding.UTF8.GetBytes(password);

            var sha = new SHA256Managed();
            var hashBytes = sha.ComputeHash(bytes);

            var hash = Convert.ToBase64String(hashBytes);

            if (hash == user.Password)
                return RedirectToAction("CompleteEvent", "Events", new {id});
            ModelState.AddModelError("", "Password incorrect");
            return View();
        }

        public ActionResult Register(Guid id)
        {
            if (isAuthenticated())
                return View();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(
            [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber,Email,FirstGraduationYear,SecondGraduationYear,ThirdGraduationYear,BirthDate,UpdateTimeStamp,ConstituentType,AllowCommunication,HasAttendedEvent,EventsAttended"
             )] Student student,
            Guid id)
        {
            string thankYou = "Thank you for registering and signing in!";

            if (ModelState.IsValid)
            {
                var ccEvent = db.Events.Find(id);
                student.StudentId = Guid.NewGuid();
                student.HasAttendedEvent = true;
                student.EventsAttended = 1;
                student.ConstituentType = "Alumni";
                student.UpdateTimeStamp = DateTime.Now;
                db.Students.Add(student);
                AttendEvent(student.StudentId, id);
                ccEvent.Attendance++;
                db.SaveChanges();
                return RedirectToAction("SignIn", new { id = id, message = thankYou });
            }
            return View();
        }

        private bool isAuthenticated()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                if (ticket != null)
                    return true;
            }
            return false;
        }
        public void ExportEvent(Guid id)
        {
            var sw = new StringWriter();

            sw.WriteLine("\"Student Number\",\"First Name\",\"Middle Name\",\"Last Name\",\"Address1\"," +
                         "\"Address2\",\"Zip Code\",\"City\",\"State\",\"Phone Number\",\"Email\",\"Graduation Year" +
                         "\"Birthday\"");
            Response.ClearContent();
            Response.AddHeader("content-disposition",
                "attachment;filename=ExportedConstituents_" + DateTime.Now + ".csv");
            Response.ContentType = "text/csv";

            var attendees =
                (from ev in db.EventAttendants
                 where (ev.EventId == id)
                 select ev).ToList();


            foreach (var attendee in attendees)
            {
                Guid sId = attendee.StudentId;
                var student = db.Students.Find(sId);
                sw.WriteLine(
                $"\"{student.StudentNumber}\",\"{student.FirstName}\",\"{student.MiddleName}\",\"{student.LastName}\",\"{student.Address1}\"," +
                $"\"{student.Address2}\",\"{student.ZipCode}\",\"{student.City}\",\"{student.State}\",\"{student.PhoneNumber}\",\"{student.Email}\"," +
                $"\"{student.FirstGraduationYear}\",\"{student.BirthDate}\"");
           }

            Response.Write(sw.ToString());
            Response.End();
        }
    }
}