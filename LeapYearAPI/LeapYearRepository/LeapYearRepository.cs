using EmployeeAPI.Models;
using LeapYearAPI.Models;
using Newtonsoft.Json;

namespace LeapYearAPI.LeapYearRepository
{
    public class LeapYearRepository : ILeapYearRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILeapYearRepository _leapYearRepository;
        private ILeapYearRepository @object;

        public LeapYearRepository(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        //public LeapYearRepository(ILeapYearRepository @object)
        //{
        //    this.@object = @object;
        //}

        public List<LeapYearResponse> GetLeapYear(LeapYearRange leapYearRange)
        {
            try
            {
                if (leapYearRange.StartYear <= 0 || leapYearRange.EndYear <= 0 || leapYearRange == null)
                {
                    throw new ArgumentException("Range is not valid , It should be positive values");
                }
                List<LeapYearResponse> yearsList = new List<LeapYearResponse>();
                if (leapYearRange != null)
                {
                    for (int year = leapYearRange.StartYear; year <= leapYearRange.EndYear; year++)
                    {
                        if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0))
                        {
                            yearsList.Add(new LeapYearResponse { LeapYear = year });
                        }
                    }
                }

                return yearsList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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
                        var leapyearDay = GetdayAccordingLeapYear(DateToGetDay, url);
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
        private static async Task<string> GetdayAccordingLeapYear(DateTime dateTime, string url)
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
