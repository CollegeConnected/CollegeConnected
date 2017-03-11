﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class Student
    {
        public Guid StudentId { get; set; }
        public string StudentNumber { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [StringLength(5)]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Zip Code must contain only numbers")]
        public string ZipCode { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstGraduationYear { get; set; }
        public string SecondGraduationYear { get; set; }
        public string ThirdGraduationYear { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public DateTime UpdateTimeStamp { get; set; }
        public string ConstituentType { get; set; }
        [Required]
        public bool AllowCommunication { get; set; }
        public bool HasAttendedEvent { get; set; }
        public int EventsAttended { get; set; }
        public string BirthdayForDisplay
        {
            get { return BirthDate.ToShortDateString(); }
        }
    }
}