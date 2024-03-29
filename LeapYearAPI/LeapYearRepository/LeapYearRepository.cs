﻿using EmployeeAPI.Models;
using LeapYearAPI.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Text;


namespace LeapYearAPI.LeapYearRepository
{
    public class LeapYearRepository : ILeapYearRepository
    {
        private readonly IConfiguration _configuration;
        private static string? _token;

        public LeapYearRepository(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public List<int> GetLeapYear(LeapYearRange leapYearRange)
        {
            try
            {
                int startYear = leapYearRange.StartYear;
                int endYear = leapYearRange.EndYear;
                //List<LeapYearResponse> yearsList = new List<LeapYearResponse>();
                List<int > years = new List<int>();
                if (leapYearRange.StartYear <= 0 || leapYearRange.EndYear <= 0 || leapYearRange == null)
                {
                    LogInformation("Error", "Range is not valid , It should be greater than  Zero");
                    throw new ArgumentException("Range is not valid , It should be positive values");
                }
                if (leapYearRange.StartYear > leapYearRange.EndYear)
                {
                    startYear = leapYearRange.EndYear;
                    endYear = leapYearRange.StartYear;
                }
                if (leapYearRange != null)
                {
                    for (int year = startYear; year <= endYear; year++)
                    {
                        if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0))
                        {
                            years.Add(year);
                        }
                    }
                }
                return years;
            }
            catch (Exception ex)
            {
                LogInformation("Error", ex.Message);
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
                LogInformation("Error", ex.Message);
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
            HttpClient httpClient = new HttpClient();
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{url}{dateTime}"),
            };
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            requestMessage.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(requestMessage);
            if (response.ReasonPhrase == "Unauthorized")
            {
                LogInformation("Error", $"{response.ReasonPhrase}  User, Please login with your username and password !") ;
                throw new Exception(response.ReasonPhrase + " " + "User, Please login with your username and password !");
            }
            string dayOfLeapYear = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                using (Stream stream = response.Content.ReadAsStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    dayOfLeapYear = sr.ReadToEnd();
                    sr.Close();
                }
                return dayOfLeapYear;
            }
            else
            {
                LogInformation("Error", $"{response.ReasonPhrase}  User, Please login with your username and password !");
                throw new Exception(response.ReasonPhrase + " " + "User, Please login with your username and password !");
            }

        }
        /// <summary>
        /// Call of Employeed Api project endpoint which will return token of authorization.
        /// </summary>
        /// <param name="logIn"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public LoginResponseModel LogInApiCall(LogInModel logIn)
        {
            try
            {
                LoginResponseModel model = new LoginResponseModel();
                using (var client = new HttpClient())
                {
                    var url = _configuration["Url:LogInUrl"];
                    client.BaseAddress = new Uri(url);

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync("LogIn", logIn);
                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        string response;
                        using (Stream stream = result.Content.ReadAsStream())
                        {
                            StreamReader sr = new StreamReader(stream);
                            response = sr.ReadToEnd();

                            sr.Close();
                            model = JsonConvert.DeserializeObject<LoginResponseModel>(response);
                            _token = model.token!;
                        }
                        LogInformation("Succes", $" {logIn.Email}  Logged In Time = {DateTime.Now}");
                        return model;
                        
                    }
                    if(!result.IsSuccessStatusCode)
                    {
                        LogInformation(result.StatusCode.ToString(), result.ReasonPhrase!);
                    }
                    return model;
                }
            }
            catch (Exception ex)
            {
                LogInformation("Error", ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private static Response LogInformation(string status, string message)
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
