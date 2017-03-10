using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class Student
    {
        public Guid StudentId { get; set; }
        public string StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FirstGraduationYear { get; set; }
        public string SecondGraduationYear { get; set; }
        public string ThirdGraduationYear { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public DateTime UpdateTimeStamp { get; set; }
        public string ConstituentType { get; set; }
        public bool AllowCommunication { get; set; }
        public bool HasAttendedEvent { get; set; }
        public int EventsAttended { get; set; }
        public string BirthdayForDisplay
        {
            get { return BirthDate.ToShortDateString(); }
        }
    }
}