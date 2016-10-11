using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class EventCreatorController : Controller
    {
        [HttpGet]
        public ViewResult CreateEventForm()
        {
            return View();
        }

        [HttpPost]
        public ViewResult CreateEventForm(EventCreator eventCreator){
            if (ModelState.IsValid)
            {
                return View("Your event has been created.", eventCreator);
            }
            else
            {
                //there is a validation error
                return View();
            }
        }
    }
}