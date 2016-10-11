using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CollegeConnected.Models
{
    public class StudentImportOptionsModel
    {
        public int RecordCount;

        [Required]
        [Display(Name = "Import File")]
        public HttpPostedFileBase File { get; set; }
    }

    public class StudentImportColumnConfigurationModel
    {
        [Display(Name = "Column Configuration")]
        public List<StudentImportColumn> Configuration { get; set; }

        public List<SelectListItem> SelectionCollection { get; set; }
    }

    public class StudentImportColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }

        [Display(Name = "Include")]
        public bool Include { get; set; }

        public List<string> sampleRowData { get; set; }
    }

    public class RejectEntry
    {
        public RejectEntry()
        {
            CellValues = new List<string>();
        }

        public string ErrorMessage { get; set; }
        public List<string> CellValues { get; set; }
    }
}