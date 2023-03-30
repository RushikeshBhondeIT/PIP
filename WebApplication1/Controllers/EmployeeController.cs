using EmployeeServiceContracts.DTO.Enums;
using EmployeeServiceContracts.DTO;
using EmployeeServiceContracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using EmployeeAPI.Models;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using Serilog;

namespace EmployeeAPI.Controllers
{

    [ApiController]
    [Authorize(Roles = "Admin,HR")]
    [RoutePrefix("api/v1/Employee/")]
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


        [HttpPost("AddCountries")]
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            try
            {
                var country= _countriesService.AddCountry(countryAddRequest);
                return country;
            }
            catch (Exception ex)
            {
                GenarateResponse("Error", ex.Message);
                throw new Exception(ex.Message);
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
            catch (Exception ex) { GenarateResponse("Error", ex.Message); throw new Exception(ex.Message); }
        }



        [HttpGet("GetAllEmployees")]
        public List<EmployeeResponse> GetAllEmployees()
        {
            try
            {
                var employees = _employeeService.GetAllEmployee().ToList();
                return employees;
            }
            catch (Exception ex) { GenarateResponse("Error", ex.Message); throw new Exception(ex.Message); }

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
            catch (Exception ex) { GenarateResponse("Error", ex.Message); throw new Exception(ex.Message); }
        }

        [HttpPut("Edit")]
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
            catch (Exception ex) { GenarateResponse("Error", ex.Message); throw new Exception(ex.Message); }
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
            }
            catch (Exception ex) { GenarateResponse("Error", ex.Message); throw new Exception(ex.Message); }
        }

        [HttpPost("GetFilteredEmployee")]
        public List<EmployeeResponse> GetFilteredEmployee(string searchBy, string? searchString)
        {
            try
            {
                List<EmployeeResponse> employee = _employeeService.GetFilteredEmployee(searchBy, searchString);
                return employee;
            }
            catch (Exception ex) { GenarateResponse("Error", ex.Message); throw new Exception(ex.Message); }

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
            catch (Exception ex) { GenarateResponse("Error", ex.Message); throw new Exception(ex.Message); }
        }

        private Response GenarateResponse(string status, string message)
        {
            Response res = new Response
            {
                Status = status,
                Message = message
            };
            if (status == "Error")
            {
                Log.Error(res.Status + " " + " " + res.Message);
            }
            else
            {
                Log.Information(res.Status + " " + " " + res.Message);
            }
            return res;
        }

    }
}

