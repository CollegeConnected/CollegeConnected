using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class Event
    {
        public string EventName { get; set; }
        public string EventLocation { get; set; }
        
        public string EventDate { get; set; }
        public string EventStartTime { get; set; }
        public string EventEndTime { get; set; }
        public string EventStatus { get; set; }
    }
}