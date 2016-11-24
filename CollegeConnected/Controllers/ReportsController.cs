using System.Web.Mvc;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
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
    }
}