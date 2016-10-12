namespace CollegeConnected.Imports
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    using CollegeConnected.Models;

    using OfficeOpenXml;

    public class StudentImport : CollegeConnectedImporterBase
    {
        private const string DuplicateStudentNumberErrorMessage =
            "Student number appears more than once in this import file.";

        public StudentImport()
        {
            options = new StudentImportOptionsModel();
            columnConfiguration = new StudentImportColumnConfigurationModel();
        }

        public StudentImportColumnConfigurationModel ColumnConfiguration
        {
            get
            {
                return columnConfiguration as StudentImportColumnConfigurationModel;
            }
        }

        public override string ColumnPage
        {
            get
            {
                return "StudentColumnOptions";
            }
        }

        public StudentImportOptionsModel Options
        {
            get
            {
                return options as StudentImportOptionsModel;
            }
        }

        public override string StartPage
        {
            get
            {
                return "StartStudent";
            }
        }

        public override bool AddColumnConfiguration(object columnConfiguration)
        {
            if (!(columnConfiguration is StudentImportColumnConfigurationModel))
            {
                return false;
            }
            var StudentOptions = columnConfiguration as StudentImportColumnConfigurationModel;
            bool StudentNumberDefined = false;

            foreach (StudentImportColumn column in StudentOptions.Configuration)
            {
                if (column.Include == false)
                {
                    continue;
                }

                if (column.Type == "Student Number")
                {
                    if (StudentNumberDefined)
                    {
                        MvcApplication.CurrentImport.ErrorMessage = "Cannot have multiple Student Numbers defined";

                        return false;
                    }
                    StudentNumberDefined = true;
                }
            }

          
            
            ColumnConfiguration.Configuration = StudentOptions.Configuration;
            return true;
        }
        private string GetStudentNumber(ExcelWorksheet worksheet, int rowIndex)
        {
            string StudentNumber = string.Empty;
            int columnCount = ColumnConfiguration.Configuration.Count();
            int excelColumnIndex = 0;
            for (int ii = 0; ii < columnCount; ii++)
            {
                excelColumnIndex = ii + 1;
                if (ColumnConfiguration.Configuration[ii].Type == "Student Number"
                    && ColumnConfiguration.Configuration[ii].Include)
                {
                    StudentNumber = worksheet.Cells[rowIndex, excelColumnIndex].RichText.Text;
                    break;
                }
            }

            return StudentNumber;
        }

        public override IEnumerable<object> ConvertData()
        {
            var convertedStudents = new List<Student>();
            string extension = Path.GetExtension(Options.File.FileName);
            if (extension == ".xls" || extension == ".xlsx")
            {
                var importFileInfo = new FileInfo(UploadPath);
                using (var package = new ExcelPackage(importFileInfo))
                {
                    var importProgressItem = ImportProgressTypeEnum.ConvertError;
                    HashSet<string> processedStudentNumbers = new HashSet<string>();
                    for (int rowIndex = 2; rowIndex <= package.Workbook.Worksheets[1].Dimension.End.Row; rowIndex++)
                    {
                        try
                        {
                            string StudentNumber = GetStudentNumber(
                                package.Workbook.Worksheets[1],
                                rowIndex);
                            if (StudentNumberAlreadyProcessed(processedStudentNumbers, StudentNumber))
                            {
                                AddConversionError(
                                    DuplicateStudentNumberErrorMessage,
                                    package.Workbook.Worksheets[1],
                                    rowIndex);
                                continue;
                            }
                            

                            var convertedStudent = new Student();
                            convertedStudent.StudentNumber = StudentNumber;


                            convertedStudents.Add(convertedStudent);

                            importProgressItem = ImportProgressTypeEnum.Converted;
                            AddProgressItem(importProgressItem);
                            processedStudentNumbers.Add(StudentNumber);
                        }
                        catch (Exception e)
                        {
                            importProgressItem = ImportProgressTypeEnum.ConvertError;
                            AddProgressItem(importProgressItem, e, package.Workbook.Worksheets[1], rowIndex);
                        }
                    }
                }
            }
            return convertedStudents;
        }

        public override void ImportData(IEnumerable<object> convertedData)
        {
            List<Student> convertedStudents = convertedData.Cast<Student>().ToList();
            using (var db = new CollegeConnectedDbContext())
            {
               //TO DO
            }
        }
        public override bool InitializeImportOptions(object options)
        {
            this.options = options;

            string extension = Path.GetExtension(Options.File.FileName).ToLower();

            if (extension != ".csv" && extension != ".xml" && extension != ".xls" && extension != ".xlsx")
            {
                ErrorMessage = "Invalid file type";
                return false;
            }

            string directory = HttpContext.Current.Server.MapPath("~/ImportFiles");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            UploadPath = Path.Combine(directory, Path.GetFileName(Options.File.FileName));
            Options.File.SaveAs(UploadPath);

            var itemList = new List<SelectListItem>();

            itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem { Text = "First Name", Value = "First Name" });
            itemList.Add(new SelectListItem { Text = "Last Name", Value = "Last Name" });
            itemList.Add(new SelectListItem { Text = "Birthdate", Value = "Birthdate" });
            itemList.Add(new SelectListItem { Text = "Address1", Value = "Address1" });
            itemList.Add(new SelectListItem { Text = "Address2", Value = "Address2" });
            itemList.Add(new SelectListItem { Text = "ZipCode", Value = "ZipCode" });
            itemList.Add(new SelectListItem { Text = "State", Value = "State" });
            itemList.Add(new SelectListItem { Text = "PhoneNumber", Value = "Addtess2" });
            itemList.Add(new SelectListItem { Text = "Email", Value = "Email" });
            itemList.Add(new SelectListItem { Text = "Graduation Year", Value = "Graduation Year" });
            itemList.Add(new SelectListItem { Text = "Student Number", Value = "Student Number" });
            itemList.Add(new SelectListItem { Text = "Student Title", Value = "Student Title" });

            ColumnConfiguration.SelectionCollection = itemList;
            return true;
        }

        public override void PrepareHeaders()
        {
            string extension = Path.GetExtension(Options.File.FileName);
            if (extension == ".xlsx" || extension == ".xls")
            {
                var importFileInfo = new FileInfo(UploadPath);
                using (var package = new ExcelPackage(importFileInfo))
                {
                    StudentImportColumn configurationColumn;

                    ColumnConfiguration.Configuration = new List<StudentImportColumn>();
                    for (int columnIndex = 1;
                         columnIndex <= package.Workbook.Worksheets[1].Dimension.End.Column;
                         columnIndex++)
                    {
                        configurationColumn = new StudentImportColumn();
                        configurationColumn.Name = package.Workbook.Worksheets[1].Cells[1, columnIndex].RichText.Text;
                        configurationColumn.Include = true;
                        configurationColumn.sampleRowData = new List<string>();

                        for (int rowIndex = 2; rowIndex <= 4; rowIndex++)
                        {
                            configurationColumn.sampleRowData.Add(
                                package.Workbook.Worksheets[1].Cells[rowIndex, columnIndex].Text);
                        }
                        ColumnConfiguration.Configuration.Add(configurationColumn);
                    }
                    Options.RecordCount = package.Workbook.Worksheets[1].Dimension.End.Row - 1;
                }
            }
        }

        public override void SetInitialRecordCount()
        {
            SetExpectedConvertedRecrods(Options.RecordCount);
        }

        protected override string[] GetColumnHeaders()
        {
            return ColumnConfiguration.Configuration.Select(x => x.Name).ToArray();
        }

        private bool StudentNumberAlreadyProcessed(HashSet<string> processedStudentNumbers, string StudentNumber)
        {
            return processedStudentNumbers.Contains(StudentNumber);
        }

        private string GetStudentTitle(ExcelWorksheet worksheet, int rowIndex)
        {
            string StudentTitle = string.Empty;
            for (int ii = 0; ii < ColumnConfiguration.Configuration.Count(); ii++)
            {
                if (ColumnConfiguration.Configuration[ii].Type == "Student Title"
                    && ColumnConfiguration.Configuration[ii].Include)
                {
                    StudentTitle = worksheet.Cells[rowIndex, ii + 1].RichText.Text;
                    break;
                }
            }
            return StudentTitle;
        }

        private string GetFormattedValidationErrorMessages(DbEntityValidationException dbException)
        {
            StringBuilder errorMessage =
                new StringBuilder(
                    "Failed to import Student data. Data for one or more Students may be in the incorrect format:");
            errorMessage.AppendLine();
            errorMessage.Append(JoinDbValidationErrorMessages(dbException));
            return errorMessage.ToString();
        }



        private List<Student> ImportStudents(CollegeConnectedDbContext db, IEnumerable<Student> convertedData)
        {
            var newStudents = new List<Student>();
            var errorStudents = new List<Student>();
            var convertedStudents = convertedData.ToList();
            int addCount = 0;

            try
            {
                List<Student> StudentList = db.Students.ToList();
                foreach (Student convertedStudent in convertedStudents)
                {
                    try
                    {

                        IEnumerable<Student> StudentQuery =
                            StudentList.Where(m => m.StudentNumber == convertedStudent.StudentNumber);

                        if (StudentQuery.Count() == 0)
                        {
                            var importedStudent = new Student();
                            IEnumerable<Student> similarStudentQuery =
                                StudentList.Where(m => m.StudentNumber == convertedStudent.StudentNumber);
                            if (similarStudentQuery.Count() > 0)
                            {
                                importedStudent.StudentGuid = similarStudentQuery.First().StudentGuid;
                            }
                            else
                            {
                                importedStudent.StudentGuid = Guid.NewGuid();
                            }
                            importedStudent.StudentNumber = convertedStudent.StudentNumber;
                            if (!String.IsNullOrEmpty(convertedStudent.FirstName))
                            {
                                importedStudent.FirstName = convertedStudent.FirstName;
                            }
                            newStudents.Add(importedStudent);
                        }
                        else
                        {
                            Student importedStudent = StudentQuery.First();
                            if (!String.IsNullOrEmpty(convertedStudent.FirstName))
                            {
                                importedStudent.FirstName = convertedStudent.FirstName;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddProgressItem(ImportProgressTypeEnum.ImportError, ex);
                        errorStudents.Add(convertedStudent);
                    }
                }
                addCount += db.SaveChanges();
                db.Students.AddRange(newStudents);
                addCount += db.SaveChanges();
            }
            catch (DbEntityValidationException dbException)
            {
                string message = GetFormattedValidationErrorMessages(dbException);
                AddImportError(message);
                return new List<Student>();
            }
            catch (Exception ex)
            {
                AddProgressItem(ImportProgressTypeEnum.ImportError, ex);
                return new List<Student>();
            }
            finally
            {
                AddProgressItem(ImportProgressTypeEnum.Imported, addCount);
            }
            return convertedStudents.Except(errorStudents).ToList();
        }

        private string JoinDbValidationErrorMessages(DbEntityValidationException dbException)
        {
            return string.Join(
                Environment.NewLine,
                dbException.EntityValidationErrors.SelectMany(
                    ve => ve.ValidationErrors.Select(dbve => dbve.ErrorMessage)));
        }
    }
}