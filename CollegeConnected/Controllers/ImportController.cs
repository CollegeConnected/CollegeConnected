using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using CollegeConnected.Imports;
using CollegeConnected.Models;

namespace CollegeConnected.Controllers
{
    public class ImportController : Controller
    {
        public ActionResult StudentColumnOptions()
        {
            if (isAuthenticated())
            {
                if (ImportManager.IsGettingColumnOptions())
                {
                    try
                    {
                        ImportManager.PrepareHeaders();
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction(
                            "UploadError",
                            new {uploadError = ex.Message, returnAction = "StartStudent"});
                    }

                    return View(MvcApplication.CurrentImport.columnConfiguration);
                }

                return RedirectToAction(ImportManager.CurrentView());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult StudentColumnOptions(StudentImportColumnConfigurationModel id)
        {
            if (ImportManager.IsGettingColumnOptions())
            {
                if (!ImportManager.AddColumnConfiguration(id))
                {
                    ModelState.AddModelError(string.Empty, MvcApplication.CurrentImport.ErrorMessage);

                    id.SelectionCollection =
                        ((StudentImportColumnConfigurationModel) MvcApplication.CurrentImport.columnConfiguration)
                        .SelectionCollection;

                    return View(id);
                }

                ImportManager.KickOffImport();

                return RedirectToAction(ImportManager.CurrentView());
            }

            return RedirectToAction(ImportManager.CurrentView());
        }

        public void DownloadReject(int id)
        {
            var cd = new ContentDisposition {FileName = "rejectfile.xlsx", Inline = false};
            Response.Clear();
            Response.AppendHeader("Content-Disposition", cd.ToString());
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            using (var db = new CollegeConnectedDbContext())
            {
                var rejectBytes = db.ImportResults.Single(m => m.Id == id).RejectFile;
                Response.BinaryWrite(rejectBytes);
            }

            Response.End();
        }

        [HttpPost]
        public async Task<ActionResult> GetProgress()
        {
            ProgressStatus result = null;
            await Task.Run(() => { result = CollegeConnectedImporterBase.GetStatus(); });
            return Json(result);
        }


        public ActionResult Progress()
        {
            return View();
        }

        public ActionResult StartStudent()
        {
            if (isAuthenticated())
            {
                if (ImportManager.IsImportReady())
                {
                    MvcApplication.CurrentImport = new StudentImport();

                    return View();
                }

                return RedirectToAction(ImportManager.CurrentView());
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult StartStudent(StudentImportOptionsModel id)
        {
            if (isAuthenticated())
            {
                if (ImportManager.IsImportReady())
                {
                    if (ModelState.IsValid)
                    {
                        if (ImportManager.InitializeImport(id))
                            return RedirectToAction(ImportManager.CurrentView());

                        ModelState.AddModelError("import", MvcApplication.CurrentImport.ErrorMessage);
                    }

                    return View();
                }

                return RedirectToAction(ImportManager.CurrentView());
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UploadError(string uploadError, string returnAction)
        {
            ViewBag.ErrorString = $"There was an error uploading the file: {uploadError}";
            ViewBag.ActionName = returnAction;

            return View();
        }

        public ActionResult ViewImportFiles()
        {
            return View();
        }

        private bool isAuthenticated()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);

                if (ticket != null)
                    return true;
            }
            return false;
        }
    }
}