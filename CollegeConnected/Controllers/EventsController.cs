using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using CollegeConnected.DataLayer;
using CollegeConnected.Models;
using CollegeConnected.Services;

namespace CollegeConnected.Controllers
{
    public class EventsController : Controller
    {
        private readonly UnitOfWork db = new UnitOfWork();
        private readonly BaseController sharedOperations = new BaseController();

        public ActionResult Index()
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
                return View(db.EventRepository.Get());
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(Guid? id)
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var ccEvent = db.EventRepository.GetById(id);
                if (ccEvent == null)
                    return HttpNotFound();
                return View(ccEvent);
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
            [Bind(Include = "EventID,EventName,EventLocation,EventStartDateTime,EventEndDateTime,Attendance")] Event
                ccEvent)
        {
            if (ModelState.IsValid)
            {
                ccEvent.EventID = Guid.NewGuid();
                ccEvent.Attendance = 0;
                ccEvent.EventStatus = "In Progress";
                ccEvent.CreatedBy = "Administrator ";
                db.EventRepository.Insert(ccEvent);
                db.Save();
                return RedirectToAction("Index");
            }

            return View(ccEvent);
        }

        public ActionResult Edit(Guid? id)
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var ccEvent = db.EventRepository.GetById(id);
                if (ccEvent == null)
                    return HttpNotFound();
                return View(ccEvent);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
        [Bind(
                Include =
                    "EventID,EventName,EventStatus,CreatedBy,EventLocation,EventStartDateTime,EventEndDateTime,Attendance"
            )
        ] Event ccEvent)
        {
            if (ModelState.IsValid)
            {
                db.EventRepository.Update(ccEvent);
                db.Save();
                return RedirectToAction("Index");
            }
            return View(ccEvent);
        }

        public ActionResult CompleteEvent(Guid id)
        {
            var ccEvent = db.EventRepository.GetById(id);
            ccEvent.EventStatus = "Completed";
            db.Save();
            return RedirectToAction("Index");
        }

        public ActionResult ReactivateEvent(Guid id)
        {
            var ccEvent = db.EventRepository.GetById(id);
            ccEvent.EventStatus = "In Progress";
            db.Save();
            return RedirectToAction("Index");
        }

        public ActionResult SignIn(Guid id, string message)
        {
            ViewBag.Message = message;
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                var ccEvent = db.EventRepository.GetById(id);
                var eventTitle = ccEvent.EventName;
                ViewBag.Title = eventTitle;
                return View(db.StudentRepository.Get(student => student.StudentNumber == "").ToList());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult SignIn(string studentNumber, string studentLastName, Guid id)
        {
            var ccEvent = db.EventRepository.GetById(id);
            var eventTitle = ccEvent.EventName;
            ViewBag.Title = eventTitle;
            if (string.IsNullOrEmpty(studentLastName))
            {
                var studentList = db.StudentRepository.Get(student => student.StudentNumber == studentNumber).ToList();
                if (!studentList.Any())
                    ModelState.AddModelError("Error",
                        "No results found. Click the Register button to sign up for collegeConnected.");
                return View(studentList);
            }
            if (string.IsNullOrEmpty(studentNumber))
            {
                var studentList = db.StudentRepository.Get(student => student.LastName == studentLastName).ToList();
                if (!studentList.Any())
                    ModelState.AddModelError("Error",
                        "No results found. Click the Register button to sign up for collegeConnected.");
                return View(studentList);
            }
            else
            {
                var studentList = db.StudentRepository.Get(student => student.StudentNumber == studentNumber).ToList();
                if (!studentList.Any())
                    ModelState.AddModelError("Error",
                        "No results found. Click the Register button to sign up for collegeConnected.");
                return View(studentList);
            }
        }

        public ActionResult Confirm(Guid? id, Guid? eventId)
        {
            ViewBag.Years = sharedOperations.GenerateGradYearList();
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                ViewBag.EventID = eventId;
                var student = db.StudentRepository.dbSet.Single(s => s.StudentId == id);
                var ccEvent = db.EventRepository.dbSet.Single(e => e.EventID == eventId);
                var eventViewModel = new EventViewModel(student, ccEvent);
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return View(eventViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(
            [Bind(
                Include =
                    "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber," +
                    "Email,FirstGraduationYear,SecondGraduationYear,ThirdGraduationYear,BirthDate,UpdateTimeStamp,ConstituentType," +
                    "AllowCommunication,HasAttendedEvent,EventsAttended"
            )] Constituent constituent,
            Guid id)
        {
            ViewBag.Years = sharedOperations.GenerateGradYearList();
            ViewBag.EventID = id;
            var ccEvent = db.EventRepository.GetById(id);
            if (ModelState.IsValid)
            {
                var a = AttendEvent(constituent.StudentId, id);
                if (a == -1)
                {
                    ModelState.AddModelError("Error",
                        "You have already signed into this event. Click the Back to Sign In link.");
                    return View(new EventViewModel(constituent, ccEvent));
                }
                if (!constituent.HasAttendedEvent)
                    constituent.HasAttendedEvent = true;
                constituent.EventsAttended++;
                ccEvent.Attendance++;
                constituent.UpdateTimeStamp = DateTime.Now;
                db.Save();
                return RedirectToAction("SignIn", new {id, message = "Thank you for signing in."});
            }
            return View(new EventViewModel(constituent, ccEvent));
        }

        public int AttendEvent(Guid studentId, Guid eventId)
        {
            var rowExists =
                db.EventAttendanceRepository.dbSet.Any(
                    ev => ev.StudentId.Equals(studentId) && ev.EventId.Equals(eventId));
            if (rowExists)
                return -1;
            var eventAttendant = new EventAttendance(Guid.NewGuid(), studentId, eventId);
            db.EventAttendanceRepository.Insert(eventAttendant);
            db.Save();
            return 0;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Verify(Guid studentId, Guid eventId, DateTime birthDate)
        {
            var student = db.StudentRepository.GetById(studentId);
            var bday = student.BirthDate;
            ViewBag.EventID = eventId;
            if (bday == birthDate)
                return RedirectToAction("Confirm", "Events", new {id = studentId, eventId});
            ModelState.AddModelError("", "Birthday incorrect");
            var ccEvent = db.EventRepository.GetById(eventId);
            var eventViewModel = new EventViewModel(student, ccEvent);
            return View(eventViewModel);
        }

        public ActionResult Verify(Guid? id, Guid eventId)
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                ViewBag.EventID = eventId;
                var student = db.StudentRepository.GetById(id);
                var ccEvent = db.EventRepository.GetById(eventId);
                var eventViewModel = new EventViewModel(student, ccEvent);
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return View(eventViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult VerifyCompleteEvent(Guid id)
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                var ccEvent = db.EventRepository.GetById(id);
                var user = db.UserRepository.GetUser();
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
            var user = db.UserRepository.GetUser();
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
            ViewBag.Years = sharedOperations.GenerateGradYearList();
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
                return View();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(
            [Bind(
                Include =
                    "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber," +
                    "Email,FirstGraduationYear,SecondGraduationYear,ThirdGraduationYear,BirthDate,UpdateTimeStamp,ConstituentType," +
                    "AllowCommunication,HasAttendedEvent,EventsAttended"
            )] Constituent constituent,
            Guid id)
        {
            ViewBag.Years = sharedOperations.GenerateGradYearList();
            if (ModelState.IsValid)
            {
                var ccEvent = db.EventRepository.GetById(id);
                constituent.StudentId = Guid.NewGuid();
                constituent.HasAttendedEvent = true;
                constituent.EventsAttended = 1;
                constituent.UpdateTimeStamp = DateTime.Now;
                db.StudentRepository.Insert(constituent);
                AttendEvent(constituent.StudentId, id);
                ccEvent.Attendance++;
                db.Save();
                return RedirectToAction("SignIn", new {id, message = "Thank you for registering and signing in!"});
            }
            return View();
        }

        public ActionResult SendEmail(Guid id)
        {
            var attendees = db.EventAttendanceRepository.Get(ev => ev.EventId == id).ToList();
            var messageBody = string.Empty;
            string subject = $"Thank you for attending {db.EventRepository.GetById(id).EventName}";
            var recipients = new Dictionary<string, string>();
            foreach (var attendee in attendees)
            {
                var student = db.StudentRepository.GetById(attendee.StudentId);
                if (student.AllowCommunication)
                    recipients.Add(student.Email, student.FirstName);
                try
                {
                    var emailService = new EmailService();
                    foreach (var recipient in recipients)
                    {
                        messageBody =
                            $"Hello {recipient.Value},<br/><br/>{db.SettingsRepository.GetUser().EventEmailMessageBody}<br/><br/>Sincerely,<br/>The UNF Alumni Association";
                        Task.Run(async () => { await emailService.SendEmailAsync(recipient.Key, messageBody, subject); }).Wait();
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Error",
                        "An error occurred emailing the event attendees. If the problem persits, contact your system administrator." +
                        $"Exception: {e}");
                }
            }
            return RedirectToAction("Index", "Events");
        }
    }
}