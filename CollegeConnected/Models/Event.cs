using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class Event
    {
        public Guid EventID { get; set; }
        [Required]
        public string EventName { get; set; }
        [Required]
        public string EventLocation { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EventStartDateTime { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EventEndDateTime { get; set; }

        public string EventStatus { get; set; }
        public string CreatedBy { get; set; }
        public int Attendance { get; set; }
    }
}