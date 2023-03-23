using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models.Authentication.SignUp
{
    public class RegisterUser
    {

        [Required(ErrorMessage = "Name can't be blank")]
        public string EmployeeName { get; set; }


        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone can't be blank")]
        [RegularExpression("\\A[0-9]{10}\\z", ErrorMessage = "Phone number should contain numbers only")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Password can't be blank")]
        [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}",ErrorMessage = "Must contain at least one  number and one uppercase and lowercase letter, and at least 8 or more characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }   
}
