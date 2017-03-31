using System;
using System.Collections.Generic;
using System.IO;
using CollegeConnected.Models;
using OfficeOpenXml;

namespace CollegeConnected.Imports
{
    public class CollegeConnectedImporterBase
    {
        private const string RejectFileHeaderErrorMessage = "Error Message";

        public CollegeConnectedImporterBase()
        {
            ProgressStatus = new ProgressStatus();
            RejectEntries = new List<RejectEntry>();
        }

        public static ProgressStatus ProgressStatus { get; set; }

        public object columnConfiguration { get; protected set; }

        public virtual string ColumnPage { get; protected set; }

        public string ErrorMessage { get; set; }

        public Guid ImportUser { get; set; }

        public object options { get; protected set; }

        public List<RejectEntry> RejectEntries { get; set; }

        public string RejectFilePath
        {
            get
            {
                if (!string.IsNullOrEmpty(UploadPath))
                    return UploadPath.Replace(".", "-reject.");
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
            if (progress == ImportProgressTypeEnum.Converted)
            {
                ProgressStatus.ConvertedRecords++;
            }
            else if (progress == ImportProgressTypeEnum.ConvertError)
            {
                ProgressStatus.ConvertErrors++;
                if (errorParams != null && errorParams.Length == 3)
                {
                    var rejectionException = (Exception) errorParams[0];
                    var worksheet = errorParams[1] as ExcelWorksheet;
                    var rowIndex = (int) errorParams[2];
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
                    var rejectionException = (Exception) errorParams[0];
                    MvcApplication.CurrentImport.AddRejectEntry(rejectionException);
                }
            }
        }

        public static void CompleteStep(ImportProgressTypeEnum progressType)
        {
            ProgressStatus.Status = (byte) progressType;
        }

        public static ProgressStatus GetStatus()
        {
            var returnStatus = ProgressStatus;

            return returnStatus;
        }

        public static void SetExpectedConvertedRecrods(int ConvertedRecordCount)
        {
            ProgressStatus.ExpectedToConvert = ConvertedRecordCount;
        }

        public virtual bool AddColumnConfiguration(object columnConfiguration)
        {
            return false;
        }

        public void AddRejectEntry(string message)
        {
            RejectEntries.Add(new RejectEntry {CellValues = new List<string>(), ErrorMessage = message});
        }

        public void AddRejectEntry(string errorMessage, ExcelWorksheet worksheet, int rowIndex)
        {
            var cellValues = new List<string>();
            if (worksheet != null)
                for (var ii = 1; ii <= worksheet.Dimension.End.Column; ii++)
                    cellValues.Add(worksheet.Cells[rowIndex, ii].RichText.Text);
            RejectEntries.Add(new RejectEntry {CellValues = cellValues, ErrorMessage = errorMessage});
        }

        public void AddRejectEntry(Exception rejectionException, ExcelWorksheet worksheet, int rowIndex)
        {
            AddRejectEntry(
                string.Format(
                    "Error on row {0}: {1}, Details: {2}",
                    rowIndex,
                    rejectionException,
                    rejectionException.InnerException),
                worksheet,
                rowIndex);
        }

        public void AddRejectEntry(Exception rejectionException)
        {
            var cellValues = new List<string>();
            RejectEntries.Add(
                new RejectEntry
                {
                    CellValues = cellValues,
                    ErrorMessage =
                        string.Format(
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
                File.Delete(RejectFilePath);

            using (var pkg = new ExcelPackage(new FileInfo(RejectFilePath)))
            {
                var worksheet = pkg.Workbook.Worksheets.Add("College Connected Reject");
                AddHeadersToRejectFile(worksheet);
                var row = 2;
                foreach (var rejectEntry in RejectEntries)
                {
                    var col = 1;
                    worksheet.Cells[row, col].Value = rejectEntry.ErrorMessage;
                    col++;
                    foreach (var cellValue in rejectEntry.CellValues)
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
            if (progress == ImportProgressTypeEnum.Converted)
                ProgressStatus.ConvertedRecords += count;
            else if (progress == ImportProgressTypeEnum.ConvertError)
                ProgressStatus.ConvertErrors += count;
            else if (progress == ImportProgressTypeEnum.Imported)
                ProgressStatus.ImportedRecords += count;
            else if (progress == ImportProgressTypeEnum.ImportError)
                ProgressStatus.ImportErrors += count;
        }

        protected virtual string[] GetColumnHeaders()
        {
            return new string[] {};
        }

        private void AddHeadersToRejectFile(ExcelWorksheet worksheet)
        {
            var row = 1;
            var col = 1;
            worksheet.Cells[row, col].Value = RejectFileHeaderErrorMessage;
            col++;
            var headers = GetColumnHeaders();
            foreach (var header in headers)
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