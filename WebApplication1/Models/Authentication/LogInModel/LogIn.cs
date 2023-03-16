using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models.Authentication.SignIn
{
    public class LogIn
    {
        [Required(ErrorMessage ="User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }    
    }
}
