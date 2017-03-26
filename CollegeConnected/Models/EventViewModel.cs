namespace CollegeConnected.Models
{
    public class EventViewModel
    {
        public EventViewModel(Constituent constituent, Event ccEvent)
        {
            Constituent = constituent;
            Event = ccEvent;
        }

        public Constituent Constituent { get; set; }
        public Event Event { get; set; }
    }
}