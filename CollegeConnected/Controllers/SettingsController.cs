using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using CollegeConnected.DataLayer;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class SettingsController : Controller
    {
        private readonly UnitOfWork db = new UnitOfWork();
        private readonly  BaseController sharedOperations = new BaseController();

        public ActionResult Configuration()
        {
            if (sharedOperations.IsAuthenticated(Request.Cookies[FormsAuthentication.FormsCookieName]))
            {
                if (db.SettingsRepository.dbSet.Any())
                {
                    var settings = db.SettingsRepository.GetUser();
                    return View(settings);
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Configuration([Bind(
            Include =
                "Id,EmailUsername,EmailPassword,EmailHostName,EmailPort,EventEmailMessageBody"
        )] Settings settings)
        {
            if (!db.SettingsRepository.dbSet.Any())
            {
                settings.Id = Guid.NewGuid();
                db.SettingsRepository.Insert(settings);
                db.Save();
            }
            else
            {
                db.SettingsRepository.Update(settings);
                db.Save();
            }
            return RedirectToAction("Admin", "Home");
        }
    }
}