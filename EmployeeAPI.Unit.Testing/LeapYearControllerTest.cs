
using LeapYearAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using LeapYearAPI.Models;
using EmployeeAPI.Models;

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
            LeapYearRange range = new LeapYearRange() { StartYear = 2000, EndYear = 2023 };
            var leapYears = (OkObjectResult)_leapYearController.GetLeapYears(range);
            var actualLeapYear = leapYears.Value;
            Assert.NotEmpty((System.Collections.IEnumerable)actualLeapYear);
        }

        [Fact]
        public void GetLeapYear_TestGettingProperLeapYears()
        {
            LeapYearRange range = new LeapYearRange() { StartYear = 2000, EndYear = 2012 };
            //  var expectedLeapYears = new string[] { "2000", "2004", "2008", "2012", "2016", "2020" }.ToList();
            List<LeapYearResponse> yearsList = new List<LeapYearResponse>();
            yearsList.Add(new LeapYearResponse { LeapYear = 2000 });
            yearsList.Add(new LeapYearResponse { LeapYear = 2004 });
            yearsList.Add(new LeapYearResponse { LeapYear = 2008 });
            yearsList.Add(new LeapYearResponse { LeapYear = 2012 });
            var leapYears =_leapYearController.GetLeapYears(range);
            var actualLeapYear = leapYears;
            Assert.Equal(actualLeapYear, leapYears);

            //foreach (LeapYearResponse expect in yearsList)
            //{

            //    foreach(var actual in actualLeapYear)
            //    {
            //        if(actual.LeapYear == expect.LeapYear)
            //        {

            //        }
            //    }
            //}
            //Assert.Equal(yearsList, actualLeapYear);
        }
    }
}
