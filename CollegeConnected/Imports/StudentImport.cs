using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CollegeConnected.Models;
using OfficeOpenXml;

namespace CollegeConnected.Imports
{
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
            get { return columnConfiguration as StudentImportColumnConfigurationModel; }
        }

        public override string ColumnPage
        {
            get { return "StudentColumnOptions"; }
        }

        public StudentImportOptionsModel Options
        {
            get { return options as StudentImportOptionsModel; }
        }

        public override string StartPage
        {
            get { return "StartStudent"; }
        }

        public override bool AddColumnConfiguration(object columnConfiguration)
        {
            if (!(columnConfiguration is StudentImportColumnConfigurationModel))
                return false;
            var studentOptions = columnConfiguration as StudentImportColumnConfigurationModel;
            var studentNumberDefined = false;

            foreach (var column in studentOptions.Configuration)
            {
                if (column.Include == false)
                    continue;

                if (column.Type == "Student Number")
                {
                    if (studentNumberDefined)
                    {
                        MvcApplication.CurrentImport.ErrorMessage = "Cannot have multiple Student Numbers defined";

                        return false;
                    }
                    studentNumberDefined = true;
                }
            }


            ColumnConfiguration.Configuration = studentOptions.Configuration;
            return true;
        }

        private string GetStringFieldsFromImportFile(ExcelWorksheet worksheet, int rowIndex, string field)
        {
            var fieldStringToImport = string.Empty;
            var columnCount = ColumnConfiguration.Configuration.Count();
            for (var ii = 0; ii < columnCount; ii++)
            {
                var excelColumnIndex = ii + 1;
                if (ColumnConfiguration.Configuration[ii].Type == field
                    && ColumnConfiguration.Configuration[ii].Include)
                {
                    fieldStringToImport = worksheet.Cells[rowIndex, excelColumnIndex].RichText.Text;
                    break;
                }
            }
            if (string.Equals(field, "Phone Number"))
            {
                var resultString = string.Join(string.Empty,
                    Regex.Matches(fieldStringToImport, @"\d+").OfType<Match>().Select(m => m.Value));
                return resultString;
            }
            return fieldStringToImport;
        }

        private State GetState(ExcelWorksheet worksheet, int rowIndex)
        {
            var state = string.Empty;
            var columnCount = ColumnConfiguration.Configuration.Count();
            var excelColumnIndex = 0;
            for (var ii = 0; ii < columnCount; ii++)
            {
                excelColumnIndex = ii + 1;
                if (ColumnConfiguration.Configuration[ii].Type == "State"
                    && ColumnConfiguration.Configuration[ii].Include)
                {
                    state = worksheet.Cells[rowIndex, excelColumnIndex].RichText.Text;
                    break;
                }
            }
            var stateValue = (State) Enum.Parse(typeof(State), state);
            return stateValue;
        }

        private DateTime GetBirthday(ExcelWorksheet worksheet, int rowIndex)
        {
            var birthday = DateTime.Now;
            var columnCount = ColumnConfiguration.Configuration.Count();
            var excelColumnIndex = 0;
            for (var ii = 0; ii < columnCount; ii++)
            {
                excelColumnIndex = ii + 1;
                if (ColumnConfiguration.Configuration[ii].Type == "Birthday"
                    && ColumnConfiguration.Configuration[ii].Include)
                {
                    birthday = Convert.ToDateTime(worksheet.Cells[rowIndex, excelColumnIndex].RichText.Text);
                    break;
                }
            }

            return birthday;
        }

        private ConstituentType GetConstituentType(ExcelWorksheet worksheet, int rowIndex)
        {
            var type = string.Empty;
            var columnCount = ColumnConfiguration.Configuration.Count();
            var excelColumnIndex = 0;
            for (var ii = 0; ii < columnCount; ii++)
            {
                excelColumnIndex = ii + 1;
                if (ColumnConfiguration.Configuration[ii].Type == "Constituent Type"
                    && ColumnConfiguration.Configuration[ii].Include)
                {
                    type = worksheet.Cells[rowIndex, excelColumnIndex].RichText.Text;
                    break;
                }
            }
            if (string.IsNullOrEmpty(type))
                type = "Alumni";
            else if (type == "Faculty/Staff")
                type = "FacultyStaff";
            var typeValue = (ConstituentType) Enum.Parse(typeof(ConstituentType), type);
            return typeValue;
        }

        public override IEnumerable<object> ConvertData()
        {
            var convertedStudents = new List<Constituent>();
            var extension = Path.GetExtension(Options.File.FileName);
            if (extension == ".xls" || extension == ".xlsx")
            {
                var importFileInfo = new FileInfo(UploadPath);
                using (var package = new ExcelPackage(importFileInfo))
                {
                    var importProgressItem = ImportProgressTypeEnum.ConvertError;
                    var processedStudentNumbers = new HashSet<string>();
                    for (var rowIndex = 2; rowIndex <= package.Workbook.Worksheets[1].Dimension.End.Row; rowIndex++)
                        try
                        {
                            var studentNumber = GetStringFieldsFromImportFile(
                                package.Workbook.Worksheets[1],
                                rowIndex, "Student Number");
                            if (StudentNumberAlreadyProcessed(processedStudentNumbers, studentNumber))
                            {
                                AddConversionError(
                                    DuplicateStudentNumberErrorMessage,
                                    package.Workbook.Worksheets[1],
                                    rowIndex);
                                continue;
                            }
                            var firstName = GetStringFieldsFromImportFile(
                                package.Workbook.Worksheets[1],
                                rowIndex, "First Name");
                            var lastName = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "Last Name");
                            var middleName = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "Middle Name");
                            var address1 = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "Address1");
                            var address2 = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "Address2");
                            var phoneNunber = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "Phone Number");
                            var city = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "City");
                            var zipCode = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "Zip Code");
                            var state = GetState(package.Workbook.Worksheets[1],
                                rowIndex);
                            var email = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "Email");
                            var firstGradYear = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "First Graduation Year");
                            var secondGradYear = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "Second Graduation Year");
                            var thirdGradYear = GetStringFieldsFromImportFile(package.Workbook.Worksheets[1],
                                rowIndex, "Third Graduation Year");
                            var birthday = GetBirthday(package.Workbook.Worksheets[1],
                                rowIndex);
                            var constituentType = GetConstituentType(package.Workbook.Worksheets[1], rowIndex);

                            var convertedStudent = new Constituent
                            {
                                StudentNumber = studentNumber,
                                FirstName = firstName,
                                LastName = lastName,
                                MiddleName = middleName,
                                Address1 = address1,
                                Address2 = address2,
                                ZipCode = zipCode,
                                City = city,
                                State = state,
                                PhoneNumber = phoneNunber,
                                Email = email,
                                FirstGraduationYear = firstGradYear,
                                SecondGraduationYear = secondGradYear,
                                ThirdGraduationYear = thirdGradYear,
                                BirthDate = birthday,
                                ConstituentType = constituentType,
                                AllowCommunication = false
                            };


                            convertedStudents.Add(convertedStudent);

                            importProgressItem = ImportProgressTypeEnum.Converted;
                            AddProgressItem(importProgressItem);
                            processedStudentNumbers.Add(studentNumber);
                        }
                        catch (Exception e)
                        {
                            importProgressItem = ImportProgressTypeEnum.ConvertError;
                            AddProgressItem(importProgressItem, e, package.Workbook.Worksheets[1], rowIndex);
                        }
                }
            }
            return convertedStudents;
        }

        public override void ImportData(IEnumerable<object> convertedData)
        {
            var convertedStudents = convertedData.Cast<Constituent>().ToList();
            using (var db = new CollegeConnectedDbContext())
            {
                var importedStudents = ImportStudents(db, convertedStudents);
                if (importedStudents.Count > 0)
                    Console.WriteLine(importedStudents.Count);
            }
        }


        public override bool InitializeImportOptions(object options)
        {
            this.options = options;

            var extension = Path.GetExtension(Options.File.FileName).ToLower();

            if (extension != ".csv" && extension != ".xml" && extension != ".xls" && extension != ".xlsx")
            {
                ErrorMessage = "Invalid file type";
                return false;
            }

            var directory = HttpContext.Current.Server.MapPath("~/ImportFiles");

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            UploadPath = Path.Combine(directory, Path.GetFileName(Options.File.FileName));
            Options.File.SaveAs(UploadPath);

            var itemList = new List<SelectListItem>();

            itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem {Text = "Student Number", Value = "Student Number"});
            itemList.Add(new SelectListItem {Text = "First Name", Value = "First Name"});
            itemList.Add(new SelectListItem {Text = "Middle Name", Value = "Middle Name"});
            itemList.Add(new SelectListItem {Text = "Last Name", Value = "Last Name"});
            itemList.Add(new SelectListItem {Text = "Address1", Value = "Address1"});
            itemList.Add(new SelectListItem {Text = "Address2", Value = "Address2"});
            itemList.Add(new SelectListItem {Text = "City", Value = "City"});
            itemList.Add(new SelectListItem {Text = "State", Value = "State"});
            itemList.Add(new SelectListItem { Text = "Zip Code", Value = "Zip Code" });
            itemList.Add(new SelectListItem {Text = "Phone Number", Value = "Phone Number"});
            itemList.Add(new SelectListItem {Text = "Email", Value = "Email"});
            itemList.Add(new SelectListItem { Text = "Birthday", Value = "Birthday" });
            itemList.Add(new SelectListItem {Text = "First Graduation Year", Value = "First Graduation Year"});
            itemList.Add(new SelectListItem {Text = "Second Graduation Year", Value = "Second Graduation Year"});
            itemList.Add(new SelectListItem {Text = "Third Graduation Year", Value = "Third Graduation Year"});
            itemList.Add(new SelectListItem {Text = "Constituent Type", Value = "Constituent Type"});

            ColumnConfiguration.SelectionCollection = itemList;
            return true;
        }

        public override void PrepareHeaders()
        {
            var extension = Path.GetExtension(Options.File.FileName);
            if (extension == ".xlsx" || extension == ".xls")
            {
                var importFileInfo = new FileInfo(UploadPath);
                using (var package = new ExcelPackage(importFileInfo))
                {
                    StudentImportColumn configurationColumn;

                    ColumnConfiguration.Configuration = new List<StudentImportColumn>();
                    for (var columnIndex = 1;
                        columnIndex <= package.Workbook.Worksheets[1].Dimension.End.Column;
                        columnIndex++)
                    {
                        configurationColumn = new StudentImportColumn();
                        configurationColumn.Name = package.Workbook.Worksheets[1].Cells[1, columnIndex].RichText.Text;
                        configurationColumn.Include = true;
                        configurationColumn.sampleRowData = new List<string>();

                        for (var rowIndex = 2; rowIndex <= 4; rowIndex++)
                            configurationColumn.sampleRowData.Add(
                                package.Workbook.Worksheets[1].Cells[rowIndex, columnIndex].Text);
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


        private string GetFormattedValidationErrorMessages(DbEntityValidationException dbException)
        {
            var errorMessage =
                new StringBuilder(
                    "Failed to import Student data. Data for one or more Students may be in the incorrect format:");
            errorMessage.AppendLine();
            errorMessage.Append(JoinDbValidationErrorMessages(dbException));
            return errorMessage.ToString();
        }


        private List<Constituent> ImportStudents(CollegeConnectedDbContext db, IEnumerable<Constituent> convertedData)
        {
            var newStudents = new List<Constituent>();
            var errorStudents = new List<Constituent>();
            var convertedStudents = convertedData.ToList();
            var addCount = 0;

            try
            {
                var StudentList = db.Constituents.ToList();
                foreach (var convertedStudent in convertedStudents)
                    try
                    {
                        var StudentQuery =
                            StudentList.Where(m => m.StudentNumber == convertedStudent.StudentNumber);

                        if (StudentQuery.Count() == 0)
                        {
                            var importedStudent = new Constituent();
                            var similarStudentQuery =
                                StudentList.Where(m => m.StudentNumber == convertedStudent.StudentNumber);
                            if (similarStudentQuery.Count() > 0)
                                importedStudent.StudentId = similarStudentQuery.First().StudentId;
                            else
                                importedStudent.StudentId = Guid.NewGuid();
                            importedStudent.StudentNumber = convertedStudent.StudentNumber;
                            if (!string.IsNullOrEmpty(convertedStudent.FirstName))
                                importedStudent.FirstName = convertedStudent.FirstName;
                            importedStudent.FirstName = convertedStudent.FirstName;
                            importedStudent.LastName = convertedStudent.LastName;
                            importedStudent.MiddleName = convertedStudent.MiddleName;
                            importedStudent.Address1 = convertedStudent.Address1;
                            importedStudent.Address2 = convertedStudent.Address2;
                            importedStudent.ZipCode = convertedStudent.ZipCode;
                            importedStudent.City = convertedStudent.City;
                            importedStudent.State = convertedStudent.State;
                            importedStudent.PhoneNumber = convertedStudent.PhoneNumber;
                            importedStudent.Email = convertedStudent.Email;
                            importedStudent.FirstGraduationYear = convertedStudent.FirstGraduationYear;
                            importedStudent.SecondGraduationYear = convertedStudent.SecondGraduationYear;
                            importedStudent.ThirdGraduationYear = convertedStudent.ThirdGraduationYear;
                            importedStudent.BirthDate = convertedStudent.BirthDate;
                            importedStudent.ConstituentType = convertedStudent.ConstituentType;
                            importedStudent.AllowCommunication = convertedStudent.AllowCommunication;
                            importedStudent.HasAttendedEvent = false;
                            importedStudent.EventsAttended = 0;
                            importedStudent.UpdateTimeStamp = new DateTime(1900, 1, 1);
                            newStudents.Add(importedStudent);
                        }
                        else
                        {
                            var importedStudent = StudentQuery.First();
                            if (!string.IsNullOrEmpty(convertedStudent.FirstName))
                                importedStudent.FirstName = convertedStudent.FirstName;
                            importedStudent.LastName = convertedStudent.LastName;
                            importedStudent.MiddleName = convertedStudent.MiddleName;
                            importedStudent.Address1 = convertedStudent.Address1;
                            importedStudent.Address2 = convertedStudent.Address2;
                            importedStudent.ZipCode = convertedStudent.ZipCode;
                            importedStudent.City = convertedStudent.City;
                            importedStudent.State = convertedStudent.State;
                            importedStudent.PhoneNumber = convertedStudent.PhoneNumber;
                            importedStudent.Email = convertedStudent.Email;
                            importedStudent.FirstGraduationYear = convertedStudent.FirstGraduationYear;
                            importedStudent.SecondGraduationYear = convertedStudent.SecondGraduationYear;
                            importedStudent.ThirdGraduationYear = convertedStudent.ThirdGraduationYear;
                            importedStudent.BirthDate = convertedStudent.BirthDate;
                            importedStudent.ConstituentType = convertedStudent.ConstituentType;
                            importedStudent.AllowCommunication = convertedStudent.AllowCommunication;
                            importedStudent.HasAttendedEvent = false;
                            importedStudent.EventsAttended = 0;
                            importedStudent.UpdateTimeStamp = new DateTime(1900, 1, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        AddProgressItem(ImportProgressTypeEnum.ImportError, ex);
                        errorStudents.Add(convertedStudent);
                    }

                addCount += db.SaveChanges();
                db.Constituents.AddRange(newStudents);
                addCount += db.SaveChanges();
            }
            catch (DbEntityValidationException dbException)
            {
                var message = GetFormattedValidationErrorMessages(dbException);
                AddImportError(message);
                return new List<Constituent>();
            }
            catch (Exception ex)
            {
                AddProgressItem(ImportProgressTypeEnum.ImportError, ex);
                return new List<Constituent>();
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