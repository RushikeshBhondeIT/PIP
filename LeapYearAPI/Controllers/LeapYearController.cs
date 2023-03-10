
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LeapYearAPI.Controllers
{

    public class LeapYearController : Controller
    {
        /// <summary>
        /// Api to get the Leap Year of Given range of date
        /// </summary>
        /// <param name="startDate">Starting Date </param>
        /// <param name="endDate"> Ending Date</param>
        /// <returns></returns>
        [HttpGet("api/v1/LeapYears")]
        public IActionResult GetLeapYears(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<string> years = new List<string>();
                for (int year = startDate.Year; year <= endDate.Year; year++)
                {
                    if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0))
                    {
                        years.Add(year.ToString());
                    }
                }
                return Ok(years.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Returns Leap years for given range with day of that Leap year
        /// </summary>
        /// <param name="startDate"> from date </param>
        /// <param name="endDate">To date </param>
        /// <returns></returns>

        [HttpGet("api/v1/LeapYearsDay")]
        public IActionResult GetLeapYearsDay(DateTime startDate, DateTime endDate)
        {
            try
            {
                List<KeyValuePair<string, string>> leapYearWithDay = new List<KeyValuePair<string, string>>();

                for (int year = startDate.Year; year <= endDate.Year; year++)
                {
                    if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0))
                    {
                        var day = startDate.Day;
                        var month = startDate.Month;
                        DateTime DateToGetDay = new DateTime(year, month, day);// Parameterized constructor
                        var leapyearDay = GetdayAccordingLeapYear(DateToGetDay);
                        leapYearWithDay.Add(new KeyValuePair<string, string>(leapyearDay.ToString(), year.ToString()));
                    }
                }
                return Ok(leapYearWithDay.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Method to the Api from this controller project of EmployeeAPI 
        /// </summary>
        /// <param name="dateTime">Date to get the Day of that date</param>
        /// <returns></returns>
        public string GetdayAccordingLeapYear(DateTime dateTime)
        {
            try
            {
                string strurltest = string.Format($"https://localhost:7115/api/v1/GetDay?dateTime={dateTime}");

                WebRequest requestObject = WebRequest.Create(strurltest);
                requestObject.Method = "GET";
                HttpWebResponse responseObject = (HttpWebResponse)requestObject.GetResponse();
                string dayOfLeapYear = null;
                using (Stream stream = responseObject.GetResponseStream())
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
