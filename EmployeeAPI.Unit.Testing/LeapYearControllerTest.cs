
using LeapYearAPI.Models;
using EmployeeAPI.Models;
using LeapYearAPI.LeapYearRepository;
using Moq;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace EmployeeAPI.Unit.Testing
{
    public class LeapYearControllerTest
    {
        private readonly LeapYearRepository _leapYearRepository;
        private readonly Mock<ILeapYearRepository> mockLeapYearRepositoryMock = new Mock<ILeapYearRepository>();
        private readonly Mock<IConfiguration> configMock = new Mock<IConfiguration>();
        public LeapYearControllerTest()
        {
            _leapYearRepository = new LeapYearRepository(configMock.Object);
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
            //  var expectedLeapYears = new string[] { "2000", "2004", "2008", "2012", "2016", "2020" }.ToList();
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
        public  async Task GetLeapYear_TestGettingProperLe()
        {
            HttpResponseMessage httpResponseMessage = new()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };

            var mockHttpClient = GetMockedHttpClient(httpResponseMessage);

            using var application = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        ServiceDescriptor serviceDescriptor = new(typeof(ILeapYearRepository),
                            typeof(LeapYearRepository), ServiceLifetime.Scoped);
                        services.Remove(serviceDescriptor);
                        services.AddScoped<ILeapYearRepository>(S => new LeapYearRepository((IConfiguration)mockHttpClient));
                    });
                });
            var client = application.CreateClient();
            var response = await client.GetAsync("");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        public static HttpClient GetMockedHttpClient(HttpResponseMessage httpResponseMessage)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);
            var httpClient = new HttpClient(httpMessageHandlerMock.Object)
            {
                BaseAddress = new System.Uri("https://localhost:7115/GetDay?dateTime=")
            };
            return httpClient;
        }
    }
}
