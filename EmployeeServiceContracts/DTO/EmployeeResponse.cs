using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeServiceContracts.DTO
{
    public class EmployeeResponse
    {
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? Address { get; set; }
        public bool? ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        /// <summary>
        /// Compare the current object data with parameter object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true or false, indicatinf whether all
        /// employee details are matched with specified or not</returns>
        public override bool Equals(object? obj)
        {
            if(obj == null) return false;
            if(obj.GetType() != typeof(EmployeeResponse)) return false;
            EmployeeResponse employee= (EmployeeResponse)obj;
            return EmployeeId==employee.EmployeeId&& 
                EmployeeName==employee.EmployeeName&& 
                Email==employee.Email&& 
                DateOfBirth==employee.DateOfBirth&&
                Gender==employee.Gender&&
                CountryId==employee.CountryId&&
                CountryName==employee.CountryName&&
                Address==employee.Address&&
                Gender == employee.Gender &&
                ReceiveNewsLetters == employee.ReceiveNewsLetters;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
