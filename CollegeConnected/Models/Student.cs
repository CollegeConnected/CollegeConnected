using System;

namespace CollegeConnected.Models
{
    public class Student
    {
        public Guid StudentGuid { get; set; }
        public string StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string GraduationYear { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime UpdateTimeStamp { get; set; }
    }
}