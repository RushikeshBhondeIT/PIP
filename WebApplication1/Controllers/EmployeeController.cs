using EmployeeServiceContracts.DTO.Enums;
using EmployeeServiceContracts.DTO;
using EmployeeServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using EmployeeAPI.Models;

namespace EmployeeAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [System.Web.Http.RoutePrefix("api/v1/Employee/")]
    public class EmployeeController : Controller
    {
        //private fields
        private readonly IEmployeeService _employeeService;
        private readonly ICountriesService _countriesService;

        //constructor
        public EmployeeController(IEmployeeService employeeService, ICountriesService countriesService)
        {
            _employeeService = employeeService;
            _countriesService = countriesService;
        }

        //Url: employee/index
        [HttpGet("Index")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(EmployeeResponse.EmployeeName), SortOrderOption sortOrder = SortOrderOption.ASC)
        {
            try
            { //Search
                var serchFields = new Dictionary<string, string>() {
            { nameof(EmployeeResponse.EmployeeName), "Employee Name" },
            { nameof(EmployeeResponse.Email), "Email" },
            { nameof(EmployeeResponse.DateOfBirth), "Date of Birth" },
            { nameof(EmployeeResponse.Gender), "Gender" },
            { nameof(EmployeeResponse.CountryId), "Country" },
            { nameof(EmployeeResponse.Address), "Address" }
          };

                List<EmployeeResponse> persons = _employeeService.GetFilteredEmployee(searchBy, searchString);
                ViewBag.CurrentSearchBy = searchBy;
                ViewBag.CurrentSearchString = searchString;

                //Sort
                List<EmployeeResponse> sortedPersons = _employeeService.GetSoretedEmployee(persons, sortBy, sortOrder);
                return (IActionResult)sortedPersons.ToList();

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message =ex.Message });
            }
        }

        [HttpGet("GetAllCountries")]
        public List<CountryResponse> GetAllCountries()
        {
            try
            {
                var countries = _countriesService.GetAllCountries();
                return countries;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }

        [HttpPost("GetFilteredEmployee")]
        public List<EmployeeResponse> GetFilteredEmployee(string searchBy, string? searchString)
        {
            try
            {
                List<EmployeeResponse> employee = _employeeService.GetFilteredEmployee(searchBy, searchString);
                return employee;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }

        [HttpPost("GetAllEmployees")]
        public List<EmployeeResponse> GetAllEmployees()
        {
            try
            {
                var employees = _employeeService.GetAllEmployee().ToList();
                return employees;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }

        [HttpPost("GetSortedEmployee")]
        public List<EmployeeResponse> GetSortedEmployee(string searchBy, string? searchString, string sortBy = nameof(EmployeeResponse.EmployeeName), SortOrderOption sortOrder = SortOrderOption.ASC)
        {
            try
            {
                List<EmployeeResponse> employee = _employeeService.GetFilteredEmployee(searchBy, searchString);
                List<EmployeeResponse> sortedPersons = _employeeService.GetSoretedEmployee(employee, sortBy, sortOrder);
                return sortedPersons;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        [HttpPost("Create")]
        public EmployeeResponse Create(EmployeeAddRequest employeeAddRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    List<CountryResponse> countries = _countriesService.GetAllCountries();
                    var countrylist = countries.Select(temp =>
                    new SelectListItem() { Text = temp.CountryName, Value = temp.CountyId.ToString() });

                    var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    //return error;
                }
                EmployeeResponse employeeResponse = _employeeService.AddEmployee(employeeAddRequest);
                return employeeResponse;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        [HttpPost("Edit")]
        public UpdateEmployeeRequest Edit(UpdateEmployeeRequest employeeUpdateRequest)
        {
            try
            {
                EmployeeResponse? employeeResponse = _employeeService.GetEmployeeById(employeeUpdateRequest.EmployeeId);

                if (employeeResponse == null)
                {
                    BadRequest(employeeUpdateRequest);
                }

                if (ModelState.IsValid)
                {
                    EmployeeResponse updatedEmployee = _employeeService.UpdateEmployee(employeeUpdateRequest);
                    return updatedEmployee.ToEmployeeUpdateRequest();
                }
                else
                {
                    List<CountryResponse> countries = _countriesService.GetAllCountries();
                    var Countries = countries.Select(temp =>
                     new SelectListItem() { Text = temp.CountryName, Value = temp.CountyId.ToString() });

                    ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    EmployeeResponse empResponse = new EmployeeResponse();
                    return empResponse.ToEmployeeUpdateRequest();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }


        [HttpPost("Delete")]

        public bool Delete(UpdateEmployeeRequest UpdateResult)
        {
            try
            {
                EmployeeResponse? pemployeeResponse = _employeeService.GetEmployeeById(UpdateResult.EmployeeId);
                if (pemployeeResponse == null)
                    return false;

                var result = _employeeService.DeleteEmployee(UpdateResult.EmployeeId);
                return result;
            }catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}

