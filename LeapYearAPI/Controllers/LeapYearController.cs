
using EmployeeAPI.Models;
using EmployeeServicesRepo.Heplers;
using LeapYearAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace LeapYearAPI.Controllers
{
    [System.Web.Http.RoutePrefix("api/v1/")]
    public class LeapYearController : Controller
    {
        private  readonly IConfiguration _configuration;
        public LeapYearController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Api to get the Leap Year of Given range of date
        /// </summary>
        /// <param name="startDate">Starting Date </param>
        /// <param name="endDate"> Ending Date</param>
        /// <returns></returns>
        [HttpPost("LeapYears")]
        public IActionResult GetLeapYears(LeapYearRange leapYearRange)
        {
            List<LeapYearResponse> yearsList = new List<LeapYearResponse>();
            try
            {
                if (leapYearRange.StartYear == 0 && leapYearRange.EndYear == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Please Enter Valid Range like ex.(2000 and 2023)"});
                }
                else
                {
                    ValidationHelper.ModelValidation(leapYearRange);
                    if (ModelState.IsValid)
                    {
                        for (int year = leapYearRange.StartYear; year <= leapYearRange.EndYear; year++)
                        {
                            if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0))
                            {
                                yearsList.Add(new LeapYearResponse { LeapYear = year });
                            }
                        }
                    }
                }

                return Ok(yearsList);
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
                List<LeapYearDayResponse> leapYearWithDay = new List<LeapYearDayResponse>();
                for (int year = startDate.Year; year <= endDate.Year; year++)
                {
                    if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0))
                    {
                        var day = startDate.Day;
                        var month = startDate.Month;
                        DateTime DateToGetDay = new DateTime(year, month, day);// Parameterized constructor
                        var url = _configuration["Url:Path"];
                        var leapyearDay = GetdayAccordingLeapYear(DateToGetDay,url);
                        var leapYearResult = JsonConvert.DeserializeObject<Response>(leapyearDay.Result);
                        leapYearWithDay.Add(new LeapYearDayResponse() { Day = leapYearResult.Message, Year = year });
                    }
                }
                return leapYearWithDay;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Method to the Api from this controller project of EmployeeAPI 
        /// </summary>
        /// <param name="dateTime">Date to get the Day of that date</param>
        /// <returns></returns>
        private static async Task<string> GetdayAccordingLeapYear(DateTime dateTime,string url)
        {
            try
            {
                string strurltest = string.Format($"{url}{dateTime}");
                HttpClient client = new HttpClient();
                var result = await client.GetAsync(strurltest);
                string dayOfLeapYear = "";
                using (Stream stream = result.Content.ReadAsStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    dayOfLeapYear = sr.ReadToEnd();
                    sr.Close();
                }
                return dayOfLeapYear;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
