using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollegeConnected.Models
{
    public class EventViewModel
    {
        public Student Student { get; set; }
        public Event Event { get; set; }
        public EventViewModel (Student student, Event ccEvent)
        {
            Student = student;
            Event = ccEvent;
        }
    }
}