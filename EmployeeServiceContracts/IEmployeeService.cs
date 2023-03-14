using EmployeeServiceContracts.DTO;
using EmployeeServiceContracts.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeServiceContracts
{
    /// <summary>
    /// Represents business logic for manupulation person entity
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Adds a new employee into the list of employee
        /// </summary>
        /// <param name="empoyeeAddRequest"></param>
        /// <returns> Returns same employee details along with 
        /// newq;y generated EmployeeID</returns>
        EmployeeResponse AddEmployee(EmployeeAddRequest? empoyeeAddRequest);
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Return all employee form list</returns>
        List<EmployeeResponse> GetAllEmployee();
        /// <summary>
        /// Return Emplyee based on Id 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        EmployeeResponse? GetEmployeeById(Guid? employeeId);
        /// <summary>
        /// Returns All employee objects that matches withthe given search field and serch string
        /// </summary>
        /// <param name="serchBy"></param>
        /// <param name="serchString"></param>
        /// <returns></returns>
        List<EmployeeResponse> GetFilteredEmployee(string serchBy, string? serchString);

        public List<EmployeeResponse> GetSoretedEmployee(List<EmployeeResponse> employees, string sortBy, SortOrderOption options);

        public EmployeeResponse UpdateEmployee(UpdateEmployeeRequest? updateEmployeeRequest);

        public bool DeleteEmployee(Guid? id);

        public string GetServerTime();

        public string GetDay(DateTime? date);

    }
}
