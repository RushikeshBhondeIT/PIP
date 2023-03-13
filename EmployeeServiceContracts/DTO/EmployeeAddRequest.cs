using EmployeeServiceContracts.DTO.Enums;
using Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace EmployeeServiceContracts.DTO
{/// <summary>
/// Acts as DTO for inserting the Employee
/// </summary>
    public class EmployeeAddRequest
    {
        [Required(ErrorMessage ="Employee Name cant be blank")]
        public string? EmployeeName { get; set; }
        [Required(ErrorMessage = "Eamil cant be blank")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public string? CountryName { get; set; }
        public bool ReceiveNewsLetters { get; set; }


        /// <summary>
        /// Convert the current object of
        /// EmployeeAddRequest into a new object of Employee type
        /// </summary>
        /// <returns></returns>
        public Employee ToEmployee()
        {
            return new Employee
            {
                EmployeeName = EmployeeName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryID = CountryID,
                Address = Address
            };

        }
    }
    public static class EmployeeExtensions
    {/// <summary>
     /// An extension method to convert an object of emplyee class
     /// into EmployeeResponse
     /// </summary>
     /// <param name="employee"> Returns the converted employeeResponse object</param>
        public static EmployeeResponse ToEmployeeResponse(this Employee employee)
        {
            return new EmployeeResponse()
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.EmployeeName,
                Email = employee.Email,
                DateOfBirth = employee.DateOfBirth,
                CountryId = employee.CountryID,
                Gender=employee.Gender,
                Address = employee.Address,
                CountryName = employee.CountryName,
                ReceiveNewsLetters = employee.ReceiveNewsLetters,
                Age = (employee.DateOfBirth != null) ? Math.Round
                ((DateTime.Now - employee.DateOfBirth.Value).TotalDays /
                365.25) : null
            };
        }
    }
}
