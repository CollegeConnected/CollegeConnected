using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class Event
    {
        public Guid EventID { get; set; }
        public string EventName { get; set; }
        public string EventLocation { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EventStartDateTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EventEndDateTime { get; set; }
        public string EventStatus { get; set; }
        public string CreatedBy { get; set; }
        public int Attendance { get; set; }

        /*   public string DisplayEventDateAsDate
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
           }*/
    }
}