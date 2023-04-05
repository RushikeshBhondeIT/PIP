
using EmployeeAPI.Models;
using LeapYearAPI.LeapYearRepository;
using LeapYearAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Web.Http;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace LeapYearAPI.Controllers
{
    [RoutePrefix("api/v1/")]
    public class LeapYearController : Controller
    {
        private readonly ILeapYearRepository _leapYearRepository;

        public LeapYearController(ILeapYearRepository leapYearRepository)
        {
            _leapYearRepository = leapYearRepository;
        }

        [HttpPost("LogIn")]
        public LoginResponseModel LogIn([FromBody] LogInModel logIn)
        {
            try
            {
                return _leapYearRepository.LogInApiCall(logIn);
            }
            catch (Exception ex)
            {
                LogError("Error", ex.Message);
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Api to get the Leap Year of Given range of date
        /// </summary>
        /// <param name="startDate">Starting Date </param>
        /// <param name="endDate"> Ending Date</param>
        /// <returns></returns>
        [HttpGet("LeapYears")]
        public List<int> GetLeapYears(LeapYearRange leapYearRange)
        {
            try
            {
                return _leapYearRepository.GetLeapYear(leapYearRange);
            }
            catch (Exception ex)
            {
                LogError("Error", ex.Message);
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Returns Leap years for given range with day of that Leap year
        /// </summary>
        /// <param name="startDate"> from date </param>
        /// <param name="endDate">To date </param>
        /// <returns></returns>

        [HttpGet("LeapYearsDay")]
        public List<LeapYearDayResponse> GetLeapYearsDay(DateTime startDate, DateTime endDate)
        {
            try
            {
                return _leapYearRepository.GetLeapYearsDay(startDate, endDate);
            }
            catch (Exception ex)
            {
                LogError("Error",ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private Response LogError(string status, string message)
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
