using EmployeeAPI.Controllers;

using Microsoft.AspNetCore.Mvc;


namespace EmployeeAPI.Unit.Testing
{
    public class ServerInformationControllerTest
    {
        private readonly ServerInformationControllers _serverInformationControllers;
        public ServerInformationControllerTest()
        {
            _serverInformationControllers = new ServerInformationControllers();
        }

        [Fact]
        public void Can_GetServerTime()
        {
            var result = _serverInformationControllers.GetServerTime();
            Assert.NotNull(result);
        }

        [Fact]
        public void Can_GetDay_IfDateProvides()
        {
            DateTime DateToGetDay = new DateTime(2023, 03, 09);
            var ApiResponse = (OkObjectResult)_serverInformationControllers.GetDay(DateToGetDay);
            var result = ApiResponse.Value;
            var IsTrue = result.Equals("Provided Date Day Is = Thursday");
            Assert.True(IsTrue);
        }

        [Fact]
        public void Cant_GetDay_IfDateNotProvides()
        {
            DateTime? DateToGetDay = null;
            var ApiResponse = (BadRequestObjectResult)_serverInformationControllers.GetDay(DateToGetDay);
            var result = ApiResponse.Value;
            var IsTrue = result.Equals("DateTime not provided");
            Assert.True(IsTrue);
        }
        
    }
}
