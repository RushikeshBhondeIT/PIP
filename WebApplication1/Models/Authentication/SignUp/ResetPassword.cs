
using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models.Authentication.SignUp
{
    public class ResetPassword
    {
        [Required]
        public string Password { get; set; } = null!;
        [Compare("Password", ErrorMessage = "The Password and confirm password does not match")]
        public string PasswordConfirmation { get; set; } = null!;
        public string EMail { get; set; } = null!;
        public string Token { get; set; } = null!;

    }
}
