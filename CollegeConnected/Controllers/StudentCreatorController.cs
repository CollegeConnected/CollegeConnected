using System.Web.Mvc;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class StudentCreatorController : Controller
    {
        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Index(StudentCreator eventCreator)
        {
            if (ModelState.IsValid)
                return View("Your Information has now been updated!", eventCreator);
            return View();
        }
    }
}