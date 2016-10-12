using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models

{
    public class StudentCreator
    {
       
        [Required(ErrorMessage = "Please enter your student N number")]
        public string StudentNumber { get; set; }
        [Required(ErrorMessage = "Please enter your First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter your Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter your Address")]
        public string Address1 { get; set; }
        [Required(ErrorMessage = "Please enter your Zip Code")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "Please select a State")]
        public string State { get; set; }
        [Required(ErrorMessage = "Please enter your phone number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter your E-mail")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter the year of graduation")]
        public string GraduationYear { get; set; }
        [Required(ErrorMessage = "Please enter your birthdate")]
        public DateTime BirthDate { get; set; }
        


    }
}