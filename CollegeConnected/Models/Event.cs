using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class Event
    {
        public Guid EventID { get; set; }
        public string EventName { get; set; }
        public string EventLocation { get; set; }

        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        public DateTime EventStartTime { get; set; }
        public DateTime EventEndTime { get; set; }
        public string EventStatus { get; set; }
        public string CreatedBy { get; set; }

        public string DisplayEventDateAsDate
        {
            get { return EventDate.ToShortDateString(); }
        }

        public string DisplayEventStartTimeAsTime
        {
            get { return EventStartTime.ToShortTimeString(); }
        }

        public string DisplayEventEndTimeAsTime
        {
            get { return EventEndTime.ToShortTimeString(); }
        }
    }
}