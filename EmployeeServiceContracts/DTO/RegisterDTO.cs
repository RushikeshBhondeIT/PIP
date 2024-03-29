﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeServiceContracts.DTO
{
    public  class RegisterDTO
    {
        [Required(ErrorMessage = "Name can't be blank")]
        public string EmployeeName { get; set; }


        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address format")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phone can't be blank")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^\\d{4}\\d{3}\\d{4}$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Password can't be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm Password can't be blank")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
