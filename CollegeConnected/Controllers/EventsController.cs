using System;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class EventsController : Controller
    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();

        // GET: Events http://stackoverflow.com/questions/18237945/how-to-edit-save-viewmodels-data-back-to-database
        public ActionResult Index()
        {
            return View(db.Events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var @event = db.Events.Find(id);
            if (@event == null)
                return HttpNotFound();
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "EventID,EventName,EventLocation,EventStartDateTime,EventEndDateTime")] Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.EventID = Guid.NewGuid();
                @event.EventStatus = "In Progress";
                @event.CreatedBy = "Administrator ";
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var @event = db.Events.Find(id);
            if (@event == null)
                return HttpNotFound();
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "EventID,EventName,EventLocation,EventStartDateTime,EventEndDateTime")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var @event = db.Events.Find(id);
            if (@event == null)
                return HttpNotFound();
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var @event = db.Events.Find(id);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
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

        public ActionResult SignIn(Guid id)
        {
            string searchString = "";
            var studentList = (from student in db.Students
                               where student.StudentNumber == searchString
                               select student
                   ).ToList();
            return View(studentList);
        }
        [HttpPost]
        public ActionResult SignIn(string studentNumber, string studentLastName, Guid id)
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
        public ActionResult Confirm(Guid? id, Guid? eventId)
        {
            Student student = db.Students.Single(x => x.StudentId == id);
            Event ccEvent = db.Events.Single(x => x.EventID == eventId);
            var eventViewModel = new EventViewModel(student, ccEvent);
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (eventViewModel == null)
                return HttpNotFound();
             return View(eventViewModel);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(
            [Bind(
                 Include =
                     "StudentId,StudentNumber,FirstName,MiddleName,LastName,Address1,Address2,ZipCode,City,State,PhoneNumber,Email,FirstGraduationYear,SecondGraduationYear,ThirdGraduationYear,BirthDate,UpdateTimeStamp,ConstituentType,AllowCommunication"
             )] Student student,
            [Bind(
                 Include =
                     "EventId"
             )] Event e)
        {
            if (ModelState.IsValid)
            {
                student.UpdateTimeStamp = DateTime.Now;
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                AttendEvent(student.StudentId, e.EventID);
                return RedirectToAction("SignIn", new { id = e.EventID });
            }
            return View();
        }

        public void AttendEvent(Guid studentId, Guid eventId)
        {

            var eventAttendant = new EventAttendance(Guid.NewGuid(), studentId, eventId);
            db.EventAttendants.Add(eventAttendant);
            db.SaveChanges();

        }
    }
}