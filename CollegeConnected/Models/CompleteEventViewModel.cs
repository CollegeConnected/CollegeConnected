using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class CompleteEventViewModel
    {
        public CompleteEventViewModel(Event ccevent, string password)
        {
            Event = ccevent;
            Password = password;
        }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public Event Event { get; set; }
    }
}