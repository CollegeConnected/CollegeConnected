using System;
using System.Linq;
using System.Web.Mvc;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class HomeController : Controller
    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();

        public ActionResult Admin()
        {
            int count = db.Students.Count();
            DateTime today = DateTime.Now;
            DateTime yearAgo = today.AddDays(-365);
            int sinceCount =
            (from row in db.Students
                where row.UpdateTimeStamp >= yearAgo && row.UpdateTimeStamp <= today
                select row).Count();
            ViewBag.Count = count;
            ViewBag.sinceCount = sinceCount;

            ViewBag.Message = "Your Admin Home Page.";

            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        /*  public ActionResult Report()
        {
            ReportViewer rptViewer = new ReportViewer();

            // ProcessingMode will be Either Remote or Local  
            rptViewer.ProcessingMode = ProcessingMode.Remote;
            rptViewer.SizeToReportContent = true;
            rptViewer.ZoomMode = ZoomMode.PageWidth;
            rptViewer.Width = Unit.Percentage(99);
            rptViewer.Height = Unit.Pixel(1000);
            rptViewer.AsyncRendering = true;
            rptViewer.ServerReport.ReportServerUrl = new Uri("http://localhost/ReportServer/");

            rptViewer.ServerReport.ReportPath

            ViewBag.ReportViewer = rptViewer;
            return View();
        }*/
    }
}