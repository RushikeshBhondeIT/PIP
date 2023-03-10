using EmployeeServiceContracts.DTO.Enums;
using EmployeeServiceContracts.DTO;
using EmployeeServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceStack.Web;
using System.Collections.Generic;
using System.Globalization;

namespace EmployeeAPI.Controllers
{
    [ApiController]
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

        //Url: persons/index
        [HttpGet("api/v1/Employee/Index")]
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(EmployeeResponse.EmployeeName), SortOrderOption sortOrder = SortOrderOption.ASC)
        {
            //Search
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
            List<EmployeeResponse> sortedPersons =  _employeeService.GetSoretedEmployee(persons, sortBy, sortOrder);
            return (IActionResult)sortedPersons.ToList();
        }

        [HttpGet("api/v1/Employee/GetAllCountries")]
        public List<CountryResponse> GetAllCountries()
        {
          var countries=_countriesService.GetAllCountries();
            return countries;
        }

        [HttpPost("api/v1/Employee/GetFilteredEmployee")]
        public List<EmployeeResponse> GetFilteredEmployee(string searchBy, string? searchString)
        {
           List < EmployeeResponse > employee = _employeeService.GetFilteredEmployee(searchBy, searchString);
            return employee;
        }

        [HttpPost("api/v1/Employee/GetAllEmployees")]
        public List<EmployeeResponse> GetAllEmployees()
        {
            var employees = _employeeService.GetAllEmployee().ToList();
            return employees;
        }

        [HttpPost("api/v1/Employee/GetSortedEmployee")]
        public List<EmployeeResponse> GetSortedEmployee(string searchBy, string? searchString, string sortBy = nameof(EmployeeResponse.EmployeeName), SortOrderOption sortOrder = SortOrderOption.ASC)
        {
            List<EmployeeResponse> employee = _employeeService.GetFilteredEmployee(searchBy, searchString);
            List<EmployeeResponse> sortedPersons = _employeeService.GetSoretedEmployee(employee, sortBy, sortOrder);
            return sortedPersons;
        }

        //public List<EmployeeResponse> AddEmployeee()
        //{
        //    var employees = _employeeService.GetAllEmployee();
        //    return employees;
        //}







        //Executes when the user clicks on "Create Person" hyperlink (while opening the create view)
        //Url: persons/create
        //[HttpGet("api/v1/Employee/create")]
        //public IActionResult Create()
        //{
        //    List<CountryResponse> countries = _countriesService.GetAllCountries();
        //    ViewBag.Countries = countries.Select(temp =>
        //      new SelectListItem() { Text = temp.CountryName, Value = temp.CountyId.ToString() }
        //    );

        //    //new SelectListItem() { Text="Harsha", Value="1" }
        //    //<option value="1">Harsha</option>
        //    return View();
        //}


        //Url: persons/create
        [HttpPost("api/v1/Employee/Create")]
        public EmployeeResponse Create(EmployeeAddRequest employeeAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = _countriesService.GetAllCountries();
                var countrylist = countries.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountyId.ToString() });

                var error = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                //return error;
            }
            //call the service method
            EmployeeResponse employeeResponse =  _employeeService.AddEmployee(employeeAddRequest);

            //navigate to Index() action method (it makes another get request to "persons/index"
            return employeeResponse;
        }

       
        //[HttpGet("api/v1/Employee/Edit")] //Eg: /persons/edit/1
        //public IActionResult Edit(Guid emploeeId)
        //{
        //    EmployeeResponse? employeeResponse = _employeeService.GetEmployeeById(emploeeId);
        //    if (employeeResponse == null)
        //    {
        //        return BadRequest("Id is not valid");
        //    }
        //    EmployeeResponse empResponse= new EmployeeResponse();
        //    UpdateEmployeeRequest employeeUpdateRequest = empResponse.ToEmployeeUpdateRequest();

        //    List<CountryResponse> countries = _countriesService.GetAllCountries();
        //    var countrieslist = countries.Select(temp =>
        //    new SelectListItem() { Text = temp.CountryName, Value = temp.CountyId.ToString() });

        //    return (IActionResult)employeeUpdateRequest;
        //}


        
        [HttpPost("api/v1/Employee/Edit")]
        public UpdateEmployeeRequest Edit(UpdateEmployeeRequest employeeUpdateRequest)
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


        //[HttpGet]
        //[Route("[action]/{personID}")]
        //public IActionResult Delete(Guid? employeeId)
        //{
        //    EmployeeResponse? employeeResponse = _employeeService.GetEmployeeById(employeeId);
        //    if (employeeResponse == null)
        //        return BadRequest();

        //    return (IActionResult)employeeResponse;
        //}

        [HttpPost("api/v1/Employee/Delete")]
        
        public bool Delete(UpdateEmployeeRequest UpdateResult)
        {
            EmployeeResponse? pemployeeResponse = _employeeService.GetEmployeeById(UpdateResult.EmployeeId);
            if (pemployeeResponse == null)
                return false;

            var result=_employeeService.DeleteEmployee(UpdateResult.EmployeeId);
            return result;
        }
    }
}

