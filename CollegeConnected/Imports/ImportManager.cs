using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CollegeConnected.Models;

namespace CollegeConnected.Imports
{
    public static class ImportManager
    {
        public static Guid ImportUser
        {
            get
            {
                return MvcApplication.CurrentImport.ImportUser;
            }
        }
        public static void PrepareHeaders()
        {
            // Sets up column configuration and initializes convert count
            MvcApplication.CurrentImport.PrepareHeaders();
            MvcApplication.CurrentImport.SetInitialRecordCount();
        }

        public static bool InitializeImport(object options)
        {
            if (MvcApplication.CurrentImport.InitializeImportOptions(options))
            {
                MvcApplication.CurrentImport.Status = ImportStatusEnum.GettingColumnInformation;
                MvcApplication.CurrentImport.WarningMessage = string.Empty;
                MvcApplication.CurrentImport.ErrorMessage = string.Empty;
                return true;
            }
            return false;
        }

        public static bool AddColumnConfiguration(object columnConfiguration)
        {
            return MvcApplication.CurrentImport.AddColumnConfiguration(columnConfiguration);
        }

        public static void KickOffImport()
        {
            //MvcApplication.CurrentImport.ImportUser = UserId;
            MvcApplication.CurrentImport.Status = ImportStatusEnum.Running;

            CollegeConnectedImporterBase.CompleteStep(ImportProgressTypeEnum.ConvertStart);

            MvcApplication.ImportTask = Task.Run(() =>
            {
                try
                {
                    var convertedData = MvcApplication.CurrentImport.ConvertData();

                    CollegeConnectedImporterBase.CompleteStep(ImportProgressTypeEnum.ConversionCompleted);

                   MvcApplication.CurrentImport.ImportData(convertedData);

                    CollegeConnectedImporterBase.CompleteStep(ImportProgressTypeEnum.UploadingResults);
                }
                catch (Exception)
                {
                    CollegeConnectedImporterBase.CompleteStep(ImportProgressTypeEnum.UploadingResults);
                }
                finally
                {
                    try
                    {
                        using (var db = new CollegeConnectedDbContext())
                        {
                            var importCount = CollegeConnectedImporterBase.ProgressStatus.ImportedRecords;
                            var convertedCount = CollegeConnectedImporterBase.ProgressStatus.ConvertedRecords;
                            byte[] importFileBytes, rejectFileBytes;

                            importFileBytes = GetImportFileBytes(false);
                            if (MvcApplication.CurrentImport.RejectEntries.Count() > 0)
                            {
                                WriteRejectFile();
                                rejectFileBytes = GetImportFileBytes(true);
                            }
                            else
                            {
                                rejectFileBytes = new byte[0];
                            }


                            var result = new ImportResult
                            {
                                Type = "StudentImport",
                                ImportFile = importFileBytes,
                                RejectFile = rejectFileBytes,
                                ImportCount = (short) importCount,
                                ConvertCount = (short) convertedCount,
                                TimeStamp = DateTime.Now
                            };
                            db.ImportResults.Add(result);
                            db.SaveChanges();

                            if (rejectFileBytes.Length > 0)
                                CollegeConnectedImporterBase.ProgressStatus.FileId = result.Id.ToString();
                        }
                        CollegeConnectedImporterBase.CompleteStep(ImportProgressTypeEnum.ImportCompleted);
                    }
                    catch (Exception)
                    {
                        CollegeConnectedImporterBase.CompleteStep(ImportProgressTypeEnum.ImportCompleted);
                    }
                    finally
                    {
                        MvcApplication.CurrentImport.Status = ImportStatusEnum.Ready;
                    }
                }
            });
        }

        private static void WriteRejectFile()
        {
            MvcApplication.CurrentImport.WriteRejectFile();
        }

        private static byte[] GetImportFileBytes(bool GetRejectFile)
        {
            byte[] bytes = null;
            var filePath = GetRejectFile
                ? MvcApplication.CurrentImport.RejectFilePath
                : MvcApplication.CurrentImport.UploadPath;
            using (var fs = File.OpenRead(filePath))
            {
                try
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                }
                catch (Exception)
                {
                    bytes = new byte[0];
                }
            }
            return bytes;
        }

        internal static bool IsImportReady()
        {
            if (MvcApplication.CurrentImport == null)
                return true;
            if (MvcApplication.CurrentImport.Status == ImportStatusEnum.GettingColumnInformation)
            {
                MvcApplication.CurrentImport = null;

                return true;
            }
            if (MvcApplication.CurrentImport.Status == ImportStatusEnum.Ready)
                return true;
            return false;
        }

        internal static string CurrentView()
        {
            if (string.IsNullOrEmpty(MvcApplication.CurrentImport.StartPage) ||
                string.IsNullOrEmpty(MvcApplication.CurrentImport.ColumnPage))
                throw new Exception("Import Improperly defined. Pages are blank");
            if (MvcApplication.CurrentImport.Status == ImportStatusEnum.Ready)
                return MvcApplication.CurrentImport.StartPage;
            if (MvcApplication.CurrentImport.Status == ImportStatusEnum.GettingColumnInformation)
                return MvcApplication.CurrentImport.ColumnPage;
            return "Progress";
        }

        internal static bool IsGettingColumnOptions()
        {
            if (MvcApplication.CurrentImport.Status == ImportStatusEnum.GettingColumnInformation)
                if (HttpContext.Current.Request.Path.Contains(MvcApplication.CurrentImport.ColumnPage))
                    return true;
                else
                    return false;
            return false;
        }
    }
}