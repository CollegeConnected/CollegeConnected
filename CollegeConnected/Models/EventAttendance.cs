using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollegeConnected.Models
{
    public class EventAttendance
    {
        public EventAttendance(Guid studentId, Guid eventId)
        {
            StudentId = studentId;
            EventId = eventId;
        }
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid EventId { get; set; }
    }
}