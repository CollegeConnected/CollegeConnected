using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class EventCreator
    {
        [Required(ErrorMessage = "Please enter an event name")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Please enter an event location")]
        public string EventLocation { get; set; }

        [Required(ErrorMessage = "Please enter an event date")]
        public string EventDate { get; set; }

        [Required(ErrorMessage = "Please enter an event start time")]
        public string EventStartTime { get; set; }

        [Required(ErrorMessage = "Please enter an event end time")]
        public string EventEndTime { get; set; }
    }
}