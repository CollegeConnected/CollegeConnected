namespace CollegeConnected.Models
{
    public class EventViewModel
    {
        public EventViewModel(Student student, Event ccEvent)
        {
            Student = student;
            Event = ccEvent;
        }

        public Student Student { get; set; }
        public Event Event { get; set; }
    }
}