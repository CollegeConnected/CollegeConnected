using System.ComponentModel.DataAnnotations;

namespace CollegeConnected.Models
{
    public class User
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string UserID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string Name { get; set; }
    }
}