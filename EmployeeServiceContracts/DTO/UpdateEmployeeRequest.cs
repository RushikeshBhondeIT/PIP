using EmployeeServiceContracts.DTO.Enums;
using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace EmployeeServiceContracts.DTO
{
    public class UpdateEmployeeRequest
    {
        [Required]
        public Guid EmployeeId { get; set; }
        [Required(ErrorMessage = "Employee Name cant be blank")]
        public string? EmployeeName { get; set; }
        [Required(ErrorMessage = "Eamil cant be blank")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public string? CountryName { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; internal set; }


        /// <summary>
        /// Convert the current object of
        /// EmployeeAddRequest into a new object of Employee type
        /// </summary>
        /// <returns></returns>
        public Employee ToEmployee()
        {
            return new Employee
            {
                EmployeeId = EmployeeId,
                EmployeeName = EmployeeName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender,
                CountryID = CountryID,
                Address = Address
            };
        }
    }
}
