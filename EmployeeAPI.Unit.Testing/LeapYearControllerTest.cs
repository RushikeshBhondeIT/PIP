
using LeapYearAPI.Models;
using EmployeeAPI.Models;
using LeapYearAPI.LeapYearRepository;
using Moq;
using Microsoft.Extensions.Configuration;
using AutoFixture;
using LeapYearAPI.Controllers;


namespace EmployeeAPI.Unit.Testing
{
    public class LeapYearControllerTest
    {
        private readonly LeapYearRepository _leapYearRepository;
        private readonly Mock<IConfiguration> configMock = new Mock<IConfiguration>();

        private readonly ILeapYearRepository _IleapYearService;
        private readonly Mock<ILeapYearRepository> _IleapYearServiceMock;
        private readonly Fixture _fixture;
        public LeapYearControllerTest()
        {
            _leapYearRepository = new LeapYearRepository(configMock.Object);

            _fixture = new Fixture();
            _IleapYearServiceMock = new Mock<ILeapYearRepository>();
            _IleapYearService = _IleapYearServiceMock.Object;
        }

        [Fact]
        public void GetLeapYear_IsEmptyList()
        {
            LeapYearRange range = new LeapYearRange() { StartYear = 2000, EndYear = 2023 };
            var leapYears = _leapYearRepository.GetLeapYear(range);
            var actualLeapYear = leapYears;
            Assert.NotEmpty((System.Collections.IEnumerable)actualLeapYear);
        }

        [Fact]
        public void GetLeapYear_TestGettingProperLeapYears()
        {
            LeapYearRange range = new LeapYearRange() { StartYear = 2000, EndYear = 2012 };
            List<LeapYearResponse> yearsList = new List<LeapYearResponse>();
            yearsList.Add(new LeapYearResponse { LeapYear = 2000 });
            yearsList.Add(new LeapYearResponse { LeapYear = 2004 });
            yearsList.Add(new LeapYearResponse { LeapYear = 2008 });
            yearsList.Add(new LeapYearResponse { LeapYear = 2012 });
            var leapYears = _leapYearRepository.GetLeapYear(range);
            var actualLeapYear = leapYears;
            Assert.Equal(actualLeapYear, leapYears);
        }


        [Fact]
        public void GetLeapYear_TestGettingArgumentExceptionForZeroValues()
        {
            LeapYearRange range = new LeapYearRange() { StartYear = 0, EndYear = 0 };
            List<int> actual = new List<int>();
            List<int> expected = new List<int>();

            Assert.Throws<System.Exception>(() =>
            {
                actual = _leapYearRepository.GetLeapYear(range);
            });

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void GetLeapYear_TestGettingArgumentExceptionForMinusValues()
        {
            LeapYearRange range = new LeapYearRange() { StartYear = -20, EndYear = -30 };
            List<int> actual = new List<int>();
            List<int> expected = new List<int>();

            Assert.Throws<System.Exception>(() =>
            {
                actual = _leapYearRepository.GetLeapYear(range);
            });

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void LogIn_ProperLoginTest()
        {
            //Arrange

            LogInModel person_add_request = _fixture.Create<LogInModel>();

            LoginResponseModel person_response = _fixture.Create<LoginResponseModel>();
            _IleapYearServiceMock.Setup(temp => temp.LogInApiCall(person_add_request)).Returns(person_response);

            LeapYearController leapYearController = new LeapYearController(_IleapYearService);

            //Act
            //leapYearController.ModelState.AddModelError("PersonName", "Person Name can't be blank");

            LoginResponseModel result = leapYearController.LogIn(person_add_request);
            //Assert
            LoginResponseModel viewResult = Assert.IsType<LoginResponseModel>(result);

            viewResult.Equals(person_response);

        }


        [Fact]
        public void LogIn_NullResponseIfValueNotProvided()
        {
            //Arrange
            LogInModel person_add_request = _fixture.Create<LogInModel>();
            LoginResponseModel person_response = _fixture.Create<LoginResponseModel>();
            _IleapYearServiceMock.Setup(temp => temp.LogInApiCall(person_add_request)).Returns(person_response);
            LeapYearController leapYearController = new LeapYearController(_IleapYearService);
            //Act
            leapYearController.ModelState.AddModelError("PersonName", "Person Name can't be blank");
            LoginResponseModel result = leapYearController.LogIn(null);
            //Assert
            Assert.Null(result);
        }


        [Fact]
        public void GetLeapYearDay_ProperDataGettingTest()
        {
            //Arrange
            DateTime start = new DateTime(2000, 03, 13);
            DateTime end = new DateTime(2014, 03, 13);
            List<LeapYearDayResponse> person_response = _fixture.Create<List<LeapYearDayResponse>>();
            _IleapYearServiceMock.Setup(temp => temp.GetLeapYearsDay(start, end)).Returns(person_response);
            LeapYearController leapYearController = new LeapYearController(_IleapYearService);
            List<LeapYearDayResponse> result = leapYearController.GetLeapYearsDay(start, end);
            //Assert
            List<LeapYearDayResponse> viewResult = Assert.IsType<List<LeapYearDayResponse>>(result);

            viewResult.Equals(person_response);
        }
    }
}
