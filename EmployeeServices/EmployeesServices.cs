using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using EmployeeServiceContracts.DTO.Enums;
using EmployeeServicesRepo.Heplers;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;

namespace EmployeeServicesRepo
{
    public class EmployeesServices : IEmployeeService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICountriesService _countries;
     
        private ApplicationDbContext dbContext;
        private ICountriesService? countriesService;
   

        public EmployeesServices(ApplicationDbContext employeeDbContext, ICountriesService countriesService)
        {
            _db = employeeDbContext;
            _countries = countriesService;
           
        }

        public string GetServerTime()
        {
            try
            {
                var servertime = (new { servertime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) });
                return servertime.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public EmployeeResponse AddEmployee(EmployeeAddRequest? empoyeeAddRequest)
        {
           
                if (empoyeeAddRequest == null) { throw new ArgumentNullException(); }
                ValidationHelper.ModelValidation(empoyeeAddRequest);
                Employee employee = empoyeeAddRequest.ToEmployee();
                employee.EmployeeId = Guid.NewGuid();
                _db.Employees.Add(employee);
                _db.SaveChanges();
                _db.Dispose();
                //NOTICE
                return (employee.ToEmployeeResponse());
        }

        public List<EmployeeResponse> GetAllEmployee()
        {
            try
            {
                return _db.Employees.ToList()
                .Select(s => s.ToEmployeeResponse()).ToList();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _db.Dispose();
            }
            
        }

        public EmployeeResponse? GetEmployeeById(Guid? employeeId)
        {
            try
            {
                if (employeeId == null) { return null; }
                Employee? employee = _db.Employees.FirstOrDefault(temp => temp.EmployeeId == employeeId);
                if (employee == null)
                {
                    return null;
                }
                return employee.ToEmployeeResponse();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
          
        }

        List<EmployeeResponse> IEmployeeService.GetFilteredEmployee(string serchBy, string? serchString)
        {
            try
            {
                List<EmployeeResponse> allEmployee = GetAllEmployee();
                List<EmployeeResponse> matchingEmployee = allEmployee;
                if (!string.IsNullOrWhiteSpace(serchBy) && !string.IsNullOrEmpty(serchString))
                {

                    switch (serchBy)
                    {
                        case nameof(Employee.EmployeeName):
                            matchingEmployee = allEmployee.Where(temp => (string.IsNullOrEmpty(temp.EmployeeName) ? temp.EmployeeName.Contains(serchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                            break;
                        case nameof(Employee.Email):
                            matchingEmployee = allEmployee.Where(temp => (string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(serchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                            break;
                        case nameof(Employee.Gender):
                            matchingEmployee = allEmployee.Where(temp => (string.IsNullOrEmpty(temp.Gender.ToString()) ? temp.Gender.ToString().Contains(serchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                            break;
                        case nameof(Employee.CountryID):
                            matchingEmployee = allEmployee.Where(temp => (string.IsNullOrEmpty(temp.CountryName) ? temp.CountryName.Contains(serchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                            break;
                        default:
                            matchingEmployee = allEmployee;
                            break;
                    }

                }
                return matchingEmployee;

            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { _db.Dispose(); }
        }

        public List<EmployeeResponse> GetSoretedEmployee(List<EmployeeResponse> employees, string sortBy, SortOrderOption options)
        {
            try
            {
                if (string.IsNullOrEmpty(sortBy))
                {
                    return employees;
                }
                List<EmployeeResponse> sortedEmployeeS = (sortBy, options)
                    switch
                {
                    (nameof(EmployeeResponse.EmployeeName), SortOrderOption.ASC) =>
                    employees.OrderBy(temp => temp.EmployeeName, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(EmployeeResponse.EmployeeName), SortOrderOption.DSC) =>
                  employees.OrderByDescending(temp => temp.EmployeeName, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(EmployeeResponse.Email), SortOrderOption.ASC) =>
                  employees.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(EmployeeResponse.Email), SortOrderOption.DSC) =>
                   employees.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(EmployeeResponse.Gender), SortOrderOption.ASC) =>
                   employees.OrderBy(temp => temp.Gender?.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(EmployeeResponse.Gender), SortOrderOption.DSC) =>
                   employees.OrderByDescending(temp => temp.Gender?.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(EmployeeResponse.CountryName), SortOrderOption.ASC) =>
                  employees.OrderBy(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                    (nameof(EmployeeResponse.CountryName), SortOrderOption.DSC) =>
                   employees.OrderByDescending(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                    _ => employees

                };

                return sortedEmployeeS;
            }
            catch
            {
                return new List<EmployeeResponse>();
            }
            
        }

        public EmployeeResponse UpdateEmployee(UpdateEmployeeRequest? updateEmployeeRequest)
        {
            try
            {
                if (updateEmployeeRequest == null)
                {
                    throw new ArgumentNullException(nameof(Employee));
                }
                //validation
                ValidationHelper.ModelValidation(updateEmployeeRequest);
                //matching employee
                Employee? matchingEmployee = _db.Employees.FirstOrDefault(temp => temp.EmployeeId == updateEmployeeRequest.EmployeeId);
                if (matchingEmployee == null)
                {
                    throw new ArgumentException("Given person id does not exist");
                }

                //update details
                matchingEmployee.EmployeeName = updateEmployeeRequest.EmployeeName;
                matchingEmployee.Email = updateEmployeeRequest.Email;
                matchingEmployee.DateOfBirth = updateEmployeeRequest.DateOfBirth;
                matchingEmployee.CountryID = updateEmployeeRequest.CountryID;
                matchingEmployee.Address = updateEmployeeRequest.Address;
                matchingEmployee.CountryName = updateEmployeeRequest.CountryName;
                matchingEmployee.Gender = updateEmployeeRequest.Gender;
                matchingEmployee.ReceiveNewsLetters = updateEmployeeRequest.ReceiveNewsLetters;
                _db.SaveChanges();

                return matchingEmployee.ToEmployeeResponse();
            }
            catch { return new EmployeeResponse(); }
            finally { _db.Dispose(); }
            
        }

        public bool DeleteEmployee(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }
                Employee? employee = _db.Employees.FirstOrDefault(temp => temp.EmployeeId == id);
                if (employee == null)
                {
                    return false;
                }
                _db.Employees.Remove(_db.Employees.First(temp => temp.EmployeeId == id));
                _db.SaveChanges();
                _db.Dispose();
                return true;

            }
            catch { return false; }
           
        }

        public string GetDay(DateTime? dateTime)
        {
            try
            {
                if (dateTime != null)
                {
                    CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
                    DateTime? dateValue = dateTime;
                    // Display the DayOfWeek string representation
                    string? day = dateValue?.DayOfWeek.ToString();
                    Thread.CurrentThread.CurrentCulture = originalCulture;
                    return day;
                }
                else
                {
                    return "DateTime is not provided Properly";
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.Message);
            }
        }
    }
}
