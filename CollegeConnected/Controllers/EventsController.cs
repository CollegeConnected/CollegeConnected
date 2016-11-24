using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class EventsController : Controller
    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();

        // GET: Events
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
            [Bind(Include = "EventID,EventName,EventLocation,EventDate,EventStartTime,EventEndTime")] Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.EventID = Guid.NewGuid();
                @event.EventStatus = "In Progress";
                @event.CreatedBy = Guid.NewGuid();
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
            [Bind(Include = "EventID,EventName,EventLocation,EventDate,EventStartTime,EventEndTime,EventStatus")] Event
                @event)
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
    }
}