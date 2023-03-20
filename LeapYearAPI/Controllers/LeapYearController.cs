
using EmployeeAPI.Models;
using EmployeeServicesRepo.Heplers;
using LeapYearAPI.LeapYearRepository;
using LeapYearAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web.Http;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace LeapYearAPI.Controllers
{
    [RoutePrefix("api/v1/")]
    [Authorize]
    public class LeapYearController : Controller
    {
        private readonly ILeapYearRepository _leapYearRepository;

        public LeapYearController(ILeapYearRepository leapYearRepository)
        {
            _leapYearRepository = leapYearRepository;
        }

        /// <summary>
        /// Api to get the Leap Year of Given range of date
        /// </summary>
        /// <param name="startDate">Starting Date </param>
        /// <param name="endDate"> Ending Date</param>
        /// <returns></returns>
        [HttpPost("LeapYears")]
        public List<LeapYearResponse> GetLeapYears(LeapYearRange leapYearRange)
        {
            try
            {
                return _leapYearRepository.GetLeapYear(leapYearRange);
            }
            catch (Exception ex)
            {
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
                throw new Exception(ex.Message);
            }
        }
    }
}
