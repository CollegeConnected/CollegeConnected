using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CollegeConnected.Models
{
    public class CompleteEventViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public Event Event { get; set; }
        public CompleteEventViewModel(Event ccevent, string password)
        {
            Event = ccevent;
            Password = password;
        }
    }
}