using EmployeeAPI.Controllers;
using EmployeeServiceContracts.DTO;
using LeapYearAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace EmployeeAPI.Unit.Testing
{
    public class LeapYearControllerTest
    {
        private readonly LeapYearController _leapYearController;
        public LeapYearControllerTest()
        {
            _leapYearController = new LeapYearController();
        }

        [Fact]
        public void GetLeapYear_IsEmptyList()
        {
            DateTime startDate = new DateTime(2000, 01, 01);
            DateTime endDate = new DateTime(2023, 01, 01);
            var leapYears = (OkObjectResult)_leapYearController.GetLeapYears(startDate, endDate);
            var actualLeapYear = leapYears.Value;
            Assert.NotEmpty((System.Collections.IEnumerable)actualLeapYear);
        }

        [Fact]
        public void GetLeapYear_TestGettingProperLeapYears()
        {
            DateTime startDate = new DateTime(2000, 01, 01);
            DateTime endDate = new DateTime(2023, 01, 01);
            var expectedLeapYears = new string[] { "2000", "2004", "2008", "2012", "2016", "2020" }.ToList();
            var leapYears = (OkObjectResult)_leapYearController.GetLeapYears(startDate, endDate);
            var actualLeapYear = leapYears.Value;
            Assert.Equal(expectedLeapYears, actualLeapYear);
        }
    }
}
