

namespace CollegeConnected.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using CollegeConnected.Imports;
    using CollegeConnected.Models;
    public class ImportController : Controller
    {
        public ActionResult StudentColumnOptions()
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
                        new { uploadError = ex.Message, returnAction = "StartStudent" });
                }

                return View(MvcApplication.CurrentImport.columnConfiguration);
            }

            return RedirectToAction(ImportManager.CurrentView());
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
                        ((StudentImportColumnConfigurationModel)MvcApplication.CurrentImport.columnConfiguration)
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
            var cd = new ContentDisposition { FileName = "rejectfile.xlsx", Inline = false };
            Response.Clear();
            Response.AppendHeader("Content-Disposition", cd.ToString());
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            using (var db = new CollegeConnectedDbContext())
            {
                byte[] rejectBytes = db.ImportResults.Where(m => m.Id == id).Single().RejectFile;
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

        public ActionResult ImportStudentRequirements()
        {
            return View();
        }

        public ActionResult Progress()
        {
            return View();
        }

        public ActionResult Results()
        {
            return View();
        }
        
        public ActionResult StartStudent()
        {
            if (ImportManager.IsImportReady())
            {
                MvcApplication.CurrentImport = new StudentImport();

                return View();
            }

            return RedirectToAction(ImportManager.CurrentView());
        }

        [HttpPost]
        public ActionResult StartStudent(StudentImportOptionsModel id)
        {
            if (ImportManager.IsImportReady())
            {
                if (ModelState.IsValid)
                {
                    if (ImportManager.InitializeImport(id))
                    {
                        return RedirectToAction(ImportManager.CurrentView());
                    }

                    ModelState.AddModelError("import", MvcApplication.CurrentImport.ErrorMessage);
                }

                return View();
            }

            return RedirectToAction(ImportManager.CurrentView());
        }



        
        public ActionResult StartOtherFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StartOtherFile(string pleaseChangeThis)
        {
            return View();
        }
        
        public ActionResult StartWordFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StartWordFile(string pleaseChangeThis)
        {
            return View();
        }

        public ActionResult UploadError(string uploadError, string returnAction)
        {
            ViewBag.ErrorString = string.Format("There was an error uploading the file: {0}", uploadError);
            ViewBag.ActionName = returnAction;

            return View();
        }

        public ActionResult ViewImportFiles()
        {
            return View();
        }
        
        private List<SelectListItem> GetPositions()
        {
            var positionList = new List<SelectListItem>();
            positionList.Add(new SelectListItem { Text = "NONE", Value = string.Empty });
            for (int iPos = 5; iPos < 25; iPos++)
            {
                positionList.Add(new SelectListItem { Text = iPos.ToString(), Value = iPos.ToString() });
            }

            return positionList;
        }
    }
}