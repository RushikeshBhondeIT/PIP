using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using EmployeeServiceContracts.DTO.Enums;
using EmployeeServicesRepo;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin.BuilderProperties;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace EmployeeAPI.Unit.Testing
{
    public class EmployeeServiceTest
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _outputHelper;
        public Guid CountryID { get; private set; }

        public EmployeeServiceTest(ITestOutputHelper helper)
        {
            var countriesInitialData = new List<Country>() { };
            var employeeInitialData = new List<Employee>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Employees, employeeInitialData);
         
            _employeeService = new EmployeesServices(dbContext, _countriesService);
            _countriesService = new CountriesService(dbContext);
            _outputHelper = helper;
        }

        #region AddEmployee
        [Fact]
        public void AddEmployee_NullEmployee()
        {
            EmployeeAddRequest? employeeAddRequest = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                _employeeService.AddEmployee(employeeAddRequest);
            });
        }
        //When we supply null personName then throw ArgumentException error
        [Fact]
        public void AddEmployee_EmployeeNameIsNull()
        {
            EmployeeAddRequest? employeeAddRequest = new EmployeeAddRequest() { EmployeeName = null };

            Assert.Throws<ArgumentException>(() =>
            {
                _employeeService.AddEmployee(employeeAddRequest);
            });
        }
        //When We supply proper details it should insert the employee in the list , 
        [Fact]
        public void AddEmployee_ProperEmployeeDetails()
        {
            EmployeeAddRequest? employeeAddRequest = new EmployeeAddRequest()
            {

                EmployeeName = "Rushikesh",
                Email = "rushikeshitdev@gmail.com",
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender ="Male",
                CountryID = Guid.NewGuid(),
                CountryName = "USA",
                Address = "sample address",
                ReceiveNewsLetters = true
            };
            EmployeeResponse employe_response_from_add =
                _employeeService.AddEmployee(employeeAddRequest);

            List<EmployeeResponse> employees = _employeeService.GetAllEmployee().ToList();
            //_employeeService.AddEmployee(employeeAddRequest);
            Assert.True(employe_response_from_add.EmployeeId != Guid.Empty);
            Assert.Contains(employe_response_from_add.EmployeeName, employees.Select(x => x.EmployeeName));
        }
        #endregion

        #region GetEmployeeByEmployeeId
        [Fact]
        public void GetEmployeeByEmployeeId_NullEmployeeId()
        {
            Guid? employeeId = null;
            EmployeeResponse employe_response_from_get = _employeeService.GetEmployeeById(employeeId);

            Assert.Null(employe_response_from_get);
        }

        [Fact]
        public void GetEmployeeByEmployeeId_GetProperDetails()
        {
            //Act
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse country_response = _countriesService.AddCountry(country_request);

            EmployeeAddRequest employee_request = new EmployeeAddRequest()
            {
                EmployeeName = "rushikesh",
                Email = "rushikesh@gmail.com",
                Address = "sample address",
                CountryID = country_response.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };

            EmployeeResponse employe_response_from_add = _employeeService.AddEmployee(employee_request);
            EmployeeResponse? employee_response_from_get = _employeeService.GetEmployeeById(employe_response_from_add.EmployeeId);
            //Assert
            Assert.Equal(employe_response_from_add, employee_response_from_get);

        }
        #endregion

        #region GetAllEmployees
        [Fact]
        public void GetAllEmployee_EmptyList()
        {
            List<EmployeeResponse> employe_response_from_list = _employeeService.GetAllEmployee();
            Assert.Empty(employe_response_from_list);
        }

        [Fact]
        public void GetAllEmployee_AddFewEmployees()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "INDIA" };
            CountryResponse country_response1 = _countriesService.AddCountry(country_request1);
            CountryResponse country_response2 = _countriesService.AddCountry(country_request2);
            EmployeeAddRequest employee_request1 = new EmployeeAddRequest()
            {
                EmployeeName = "rushikesh",
                Email = "rushikesh@gmail.com",
                Address = "sample address",
                CountryID = country_response1.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };
            EmployeeAddRequest employee_request2 = new EmployeeAddRequest()
            {
                EmployeeName = "rushikesh",
                Email = "rushikesh@gmail.com",
                Address = "sample address",
                CountryID = country_response2.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };

            List<EmployeeAddRequest> employeeAddRequests = new List<EmployeeAddRequest>() { employee_request1, employee_request2 };
            List<EmployeeResponse> employee_response_list_from_add = new List<EmployeeResponse>();
            foreach (EmployeeAddRequest employeeAddRequest in employeeAddRequests)
            {
                EmployeeResponse employee_response = _employeeService.AddEmployee(employeeAddRequest);
                employee_response_list_from_add.Add(employee_response);
            }

            //ACT
            List<EmployeeResponse> getlist_from = _employeeService.GetAllEmployee();
            //Assert
            foreach (EmployeeResponse employe_ in employee_response_list_from_add)
            {
                Assert.Contains(employe_.EmployeeName, getlist_from.Select(x => x.EmployeeName));
            }

        }
        #endregion


        #region GetFilteredEmployee

        //If serch text is empty and serhc is Employee name , it should retunr all employee
        [Fact]
        public void GetFilteredEmployee_EmptySerchText()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "INDIA" };
            CountryResponse country_response1 = _countriesService.AddCountry(country_request1);
            CountryResponse country_response2 = _countriesService.AddCountry(country_request2);
            EmployeeAddRequest employee_request1 = new EmployeeAddRequest()
            {
                EmployeeName = "rushikesh",
                Email = "rushikesh@gmail.com",
                Address = "sample address",
                CountryID = country_response1.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };
            EmployeeAddRequest employee_request2 = new EmployeeAddRequest()
            {
                EmployeeName = "rushikesh",
                Email = "rushikesh@gmail.com",
                Address = "sample address",
                CountryID = country_response2.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };

            List<EmployeeAddRequest> employeeAddRequests = new List<EmployeeAddRequest>() { employee_request1, employee_request2 };
            List<EmployeeResponse> employee_response_list_from_add = new List<EmployeeResponse>();
            foreach (EmployeeAddRequest employeeAddRequest in employeeAddRequests)
            {
                EmployeeResponse employee_response = _employeeService.AddEmployee(employeeAddRequest);
                employee_response_list_from_add.Add(employee_response);
            }

            //ACT
            List<EmployeeResponse> getlist_from = _employeeService.GetFilteredEmployee(nameof(Employee.EmployeeName), " ");
            //Assert
            foreach (EmployeeResponse employe_ in employee_response_list_from_add)
            {
                Assert.Contains(employe_.EmployeeName, getlist_from.Select(x => x.EmployeeName));
            }

        }

        //add and serch based on employee name with some serch string . It should return employees
        [Fact]
        public void GetFilteredEmployee_WithSerchText()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "INDIA" };
            CountryResponse country_response1 = _countriesService.AddCountry(country_request1);
            CountryResponse country_response2 = _countriesService.AddCountry(country_request2);
            EmployeeAddRequest employee_request1 = new EmployeeAddRequest()
            {
                EmployeeName = "rushikesh",
                Email = "rushikesh@gmail.com",
                Address = "sample address",
                CountryID = country_response1.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };
            EmployeeAddRequest employee_request2 = new EmployeeAddRequest()
            {
                EmployeeName = "dilip",
                Email = "dilip@gmail.com",
                Address = "sample address",
                CountryID = country_response2.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };

            List<EmployeeAddRequest> employeeAddRequests = new List<EmployeeAddRequest>() { employee_request1, employee_request2 };
            List<EmployeeResponse> employee_response_list_from_add = new List<EmployeeResponse>();
            foreach (EmployeeAddRequest employeeAddRequest in employeeAddRequests)
            {
                EmployeeResponse employee_response = _employeeService.AddEmployee(employeeAddRequest);
                employee_response_list_from_add.Add(employee_response);
            }

            //ACT
            List<EmployeeResponse> getlist_from = _employeeService.GetFilteredEmployee(nameof(Employee.EmployeeName), "di");
            //Assert
            foreach (EmployeeResponse employe_ in employee_response_list_from_add)
            {
                if (employe_ != null)
                {
                    if (employe_.EmployeeName.Contains("di",
                   StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(employe_.EmployeeName, getlist_from.Select(x => x.EmployeeName));
                    }
                }

            }
        }
        #endregion


        #region sortedEmployee
        [Fact]
        public void GetSortedEmployee()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request2 = new CountryAddRequest() { CountryName = "INDIA" };
            CountryResponse country_response1 = _countriesService.AddCountry(country_request1);
            CountryResponse country_response2 = _countriesService.AddCountry(country_request2);
            EmployeeAddRequest employee_request1 = new EmployeeAddRequest()
            {
                EmployeeName = "rushikesh",
                Email = "rushikesh@gmail.com",
                Address = "sample address",
                CountryID = country_response1.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };
            EmployeeAddRequest employee_request2 = new EmployeeAddRequest()
            {
                EmployeeName = "dilip",
                Email = "dilip@gmail.com",
                Address = "sample address",
                CountryID = country_response2.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };

            List<EmployeeAddRequest> employeeAddRequests = new List<EmployeeAddRequest>() { employee_request1, employee_request2 };
            List<EmployeeResponse> employee_response_list_from_add = new List<EmployeeResponse>();
            foreach (EmployeeAddRequest employeeAddRequest in employeeAddRequests)
            {
                EmployeeResponse employee_response = _employeeService.AddEmployee(employeeAddRequest);
                employee_response_list_from_add.Add(employee_response);
            }

            //ACT
            List<EmployeeResponse> getAllEmployee = _employeeService.GetAllEmployee();
            List<EmployeeResponse> Employee_list_from_sort = _employeeService.GetSoretedEmployee(getAllEmployee, nameof(Employee.EmployeeName), SortOrderOption.DSC);
            employee_response_list_from_add = employee_response_list_from_add.OrderByDescending(temp => temp.EmployeeName).ToList();

            //Assert
            for (int i = 0; i < employee_response_list_from_add.Count; i++)
            {
                Assert.Equal(employee_response_list_from_add[i].EmployeeName, Employee_list_from_sort[i].EmployeeName);
            }
        }
        #endregion


        #region delete employee
        [Fact]
        public void DeleteEmployee_ValidEmployeeId()
        {
            //Arrange
            CountryAddRequest country_request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response1 = _countriesService.AddCountry(country_request1);
            EmployeeAddRequest employee_request1 = new EmployeeAddRequest()
            {
                EmployeeName = "rushikesh",
                Email = "rushikesh@gmail.com",
                Address = "sample address",
                CountryID = country_response1.CountyId,
                DateOfBirth = DateTime.Parse("2000-01-01"),
                Gender = "Male",
                ReceiveNewsLetters = true
            };
           

            EmployeeResponse response = _employeeService.AddEmployee(employee_request1);

            //ACT
            bool isdelete= _employeeService.DeleteEmployee(response.EmployeeId);

            //Assert
            Assert.True(isdelete);
        }

        [Fact]
        public void DeleteEmployee_InValidEmployeeId()
        {
            //ACT
            bool isdelete = _employeeService.DeleteEmployee( Guid.NewGuid());

            //Assert
            Assert.False(isdelete);
        }
        #endregion



    }
}
