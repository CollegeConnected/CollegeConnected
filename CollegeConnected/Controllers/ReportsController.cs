using System.Web.Mvc;
using CollegeConnected.Models;
using System.Web.Security;

namespace CollegeConnected.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            if (isAuthenticated())
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        // 
        // GET: /Report/ 

        public ActionResult ReportTemplate(string ReportName, string ReportDescription, int Width, int Height)
        {
            var rptInfo = new ReportInfo
            {
                ReportName = ReportName,
                ReportDescription = ReportDescription,
                ReportURL =
                    string.Format("../../Reports/ReportTemplate.aspx?ReportName={0}&Height={1}", ReportName, Height),
                Width = Width,
                Height = Height
            };

            return View(rptInfo);
        }
        private bool isAuthenticated()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                if (ticket != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}