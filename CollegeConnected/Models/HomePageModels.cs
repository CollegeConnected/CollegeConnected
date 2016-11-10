using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models

{
    public class HomePage
    {
        [Required(ErrorMessage = "Please enter your student N number")]
        public string StudentNumber { get; set; }

        [Required(ErrorMessage = "Please enter your first Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last Name")]
        public string LastName { get; set; }
    }
}