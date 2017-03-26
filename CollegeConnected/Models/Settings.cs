using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class Settings
    {
        [Key]
        public Guid Id { get; set; }
        [EmailAddress]
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }
        public string EmailHostName { get; set; }
        [StringLength(5)]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Port must contain only numbers")]
        public string EmailPort { get; set; }

        public string EventEmailMessageBody { get; set; }
    }
}