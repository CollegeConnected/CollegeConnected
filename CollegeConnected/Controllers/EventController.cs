using System.Web.Mvc;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class EventController : Controller
    {
        [HttpGet]
        public ViewResult CreateEventForm()
        {
            return View();
        }

        [HttpPost]
        public ViewResult CreateEventForm(EventCreator eventCreator)
        {
            if (ModelState.IsValid)
                return View("Your event has been created.", eventCreator);
            return View();
        }
    }
}