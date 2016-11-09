using CollegeConnected.Models;
using System.Web.Mvc;

namespace CollegeConnected.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Admin()
        {
            ViewBag.Message = "Your Admin Home Page.";

            return View();
        }

        [HttpPost]
        public ViewResult Index(StudentCreator studentCreator)
        {
            if (ModelState.IsValid)
                return View("Success", studentCreator);
            return View();
        }
    }
}