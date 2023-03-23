
using EmployeeServiceContracts;
using EmployeeServicesRepo;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Unit.Testing
{
    public class ServerInformationControllerTest
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICountriesService _countriesService;

        public ServerInformationControllerTest()
        {
            var countriesInitialData = new List<Country>() { };
            var employeeInitialData = new List<Employee>() { };
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Employees, employeeInitialData);

            _employeeService = new EmployeesServices(dbContext, _countriesService);
            _countriesService = new CountriesService(dbContext);
        }

        [Fact]
        public void Can_GetServerTime()
        {
            var result = _employeeService.GetServerTime();
            Assert.NotNull(result);
        }

        [Fact]
        public void Can_GetDay_IfDateProvides()
        {
            DateTime DateToGetDay = new DateTime(2023, 03, 09);
            var ApiResponse = _employeeService.GetDay(DateToGetDay);
            var result = ApiResponse;
            var IsTrue = result.Equals("Thursday");
            Assert.True(IsTrue);
        }

        [Fact]
        public void Cant_GetDay_IfDateNotProvides()
        {
            DateTime? DateToGetDay = null;

            var ApiResponse = _employeeService.GetDay(DateToGetDay);
            var result = ApiResponse;
            Assert.Equal("DateTime is not provided Properly", result);
        }



    }
}
