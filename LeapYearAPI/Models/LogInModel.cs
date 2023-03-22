using System.ComponentModel.DataAnnotations;

namespace LeapYearAPI.Models
{
    public class LogInModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
