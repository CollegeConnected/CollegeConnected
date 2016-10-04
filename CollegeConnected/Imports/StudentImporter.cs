using System;
using System.Collections.Generic;
using System.IO;
using CollegeConnected.Models;
using OfficeOpenXml;

namespace CollegeConnected.Imports
{
    public class StudentImporter
    {
        public const string Lock = "IMPORT.LOCK";

        private const string RejectFileHeaderErrorMessage = "Error Message";

        public StudentImporter()
        {
            ProgressStatus = new ProgressStatus();
            RejectEntries = new List<ImportModels.RejectEntry>();
        }

        public static ProgressStatus ProgressStatus { get; set; }

        public object columnConfiguration { get; protected set; }

        public virtual string ColumnPage { get; protected set; }

        public string ErrorMessage { get; set; }

        public Guid ImportUser { get; set; }

        public object options { get; protected set; }

        public List<ImportModels.RejectEntry> RejectEntries { get; set; }

        public string RejectFilePath
        {
            get
            {
                if (!String.IsNullOrEmpty(UploadPath))
                {
                    return UploadPath.Replace(".", "-reject.");
                }
                return string.Empty;
            }
        }

        public virtual string StartPage { get; protected set; }

        public ImportStatusEnum Status { get; set; }

        public string UploadPath { get; set; }

        public string WarningMessage { get; set; }

        public static void AddConversionError(string message, ExcelWorksheet worksheet, int rowIndex)
        {
            ProgressStatus.ConvertErrors++;
            MvcApplication.CurrentImport.AddRejectEntry(message, worksheet, rowIndex);
        }

        public static void AddImportError(string message)
        {
            ProgressStatus.ImportErrors++;
            MvcApplication.CurrentImport.AddRejectEntry(message);
        }

        public static void AddProgressItem(ImportProgressTypeEnum progress, params object[] errorParams)
        {
            //lock (CafrImportBase.Lock)
            //{
            if (progress == ImportProgressTypeEnum.Converted)
            {
                ProgressStatus.ConvertedRecords++;
            }
            else if (progress == ImportProgressTypeEnum.ConvertError)
            {
                ProgressStatus.ConvertErrors++;
                if (errorParams != null && errorParams.Length == 3)
                {
                    Exception rejectionException = (Exception)errorParams[0];
                    ExcelWorksheet worksheet = errorParams[1] as ExcelWorksheet;
                    int rowIndex = (int)errorParams[2];
                    MvcApplication.CurrentImport.AddRejectEntry(rejectionException, worksheet, rowIndex);
                }
            }
            else if (progress == ImportProgressTypeEnum.Imported)
            {
                ProgressStatus.ImportedRecords++;
            }
            else if (progress == ImportProgressTypeEnum.ImportError)
            {
                ProgressStatus.ImportErrors++;
                if (errorParams != null && errorParams.Length == 1)
                {
                    Exception rejectionException = (Exception)errorParams[0];
                    MvcApplication.CurrentImport.AddRejectEntry(rejectionException);
                }
            }
            //}
        }

        public static void CompleteStep(ImportProgressTypeEnum progressType)
        {
            //lock (CafrImportBase.Lock)
            //{
            ProgressStatus.Status = (byte)progressType;
            //}
        }

        public static ProgressStatus GetStatus()
        {
            ProgressStatus returnStatus;
            //lock (CafrImportBase.Lock)
            //{
            returnStatus = ProgressStatus;
            //}
            return returnStatus;
        }

        public static void SetExpectedConvertedRecrods(int ConvertedRecordCount)
        {
            //lock (CafrImportBase.Lock)
            //{
            ProgressStatus.ExpectedToConvert = ConvertedRecordCount;
            //}
        }

        public virtual bool AddColumnConfiguration(object columnConfiguration)
        {
            return false;
        }

        public void AddRejectEntry(string message)
        {
            RejectEntries.Add(new ImportModels.RejectEntry { CellValues = new List<string>(), ErrorMessage = message });
        }

        public void AddRejectEntry(string errorMessage, ExcelWorksheet worksheet, int rowIndex)
        {
            List<string> cellValues = new List<string>();
            if (worksheet != null)
            {
                for (int ii = 1; ii <= worksheet.Dimension.End.Column; ii++)
                {
                    cellValues.Add(worksheet.Cells[rowIndex, ii].RichText.Text);
                }
            }
            RejectEntries.Add(new ImportModels.RejectEntry { CellValues = cellValues, ErrorMessage = errorMessage });
        }

        public void AddRejectEntry(Exception rejectionException, ExcelWorksheet worksheet, int rowIndex)
        {
            AddRejectEntry(
                String.Format(
                    "Error on row {0}: {1}, Details: {2}",
                    rowIndex,
                    rejectionException,
                    rejectionException.InnerException),
                worksheet,
                rowIndex);
        }

        public void AddRejectEntry(Exception rejectionException)
        {
            List<string> cellValues = new List<string>();
            RejectEntries.Add(
                new ImportModels.RejectEntry
                {
                    CellValues = cellValues,
                    ErrorMessage =
                            String.Format(
                                "Import error: {0}, Details: {1}",
                                rejectionException,
                                rejectionException.InnerException)
                });
        }

        public virtual IEnumerable<object> ConvertData()
        {
            return null;
        }

        public virtual void ImportData(IEnumerable<object> convertedData)
        {
        }

        public virtual bool InitializeImportOptions(object options)
        {
            return false;
        }

        public virtual void PrepareHeaders()
        {
        }

        public virtual void SetInitialRecordCount()
        {
        }

        public void WriteRejectFile()
        {
            if (File.Exists(RejectFilePath))
            {
                File.Delete(RejectFilePath);
            }

            using (ExcelPackage pkg = new ExcelPackage(new FileInfo(RejectFilePath)))
            {
                ExcelWorksheet worksheet = pkg.Workbook.Worksheets.Add("CAFR Unlimited Reject");
                AddHeadersToRejectFile(worksheet);
                int row = 2;
                foreach (ImportModels.RejectEntry rejectEntry in RejectEntries)
                {
                    int col = 1;
                    worksheet.Cells[row, col].Value = rejectEntry.ErrorMessage;
                    col++;
                    foreach (string cellValue in rejectEntry.CellValues)
                    {
                        worksheet.Cells[row, col].Value = cellValue;
                        col++;
                    }
                    row++;
                }

                pkg.Save();
            }
        }

        protected static void AddProgressItem(ImportProgressTypeEnum progress, int count)
        {
            //lock (CafrImportBase.Lock)
            //{
            if (progress == ImportProgressTypeEnum.Converted)
            {
                ProgressStatus.ConvertedRecords += count;
            }
            else if (progress == ImportProgressTypeEnum.ConvertError)
            {
                ProgressStatus.ConvertErrors += count;
            }
            else if (progress == ImportProgressTypeEnum.Imported)
            {
                ProgressStatus.ImportedRecords += count;
            }
            else if (progress == ImportProgressTypeEnum.ImportError)
            {
                ProgressStatus.ImportErrors += count;
            }
            //}
        }

        protected virtual string[] GetColumnHeaders()
        {
            return new string[] { };
        }

        private void AddHeadersToRejectFile(ExcelWorksheet worksheet)
        {
            int row = 1;
            int col = 1;
            worksheet.Cells[row, col].Value = RejectFileHeaderErrorMessage;
            col++;
            string[] headers = GetColumnHeaders();
            foreach (string header in headers)
            {
                worksheet.Cells[row, col].Value = header;
                col++;
            }
        }
    }

    public class ProgressStatus
    {
        public int ConvertedRecords;

        public int ConvertErrors;

        public int ExpectedToConvert;

        public string FileId;

        public int ImportedRecords;

        public int ImportErrors;

        public byte Status;
    }
}
}