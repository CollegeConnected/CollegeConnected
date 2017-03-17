using System;

namespace CollegeConnected.Models
{
    public class EventAttendance
    {
        public EventAttendance()
        {

        }
        public EventAttendance(Guid id, Guid studentId, Guid eventId)
        {
            Id = id;
            StudentId = studentId;
            EventId = eventId;
        }

        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid EventId { get; set; }
    }
}