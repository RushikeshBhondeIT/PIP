using EmployeeAPI.Models;
using EmployeeServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Web.Http;
using AllowAnonymousAttribute = Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;

namespace EmployeeAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,HR")]
    [RoutePrefix("api/v1/")]
    public class ServerInformationControllers : Controller
    {
        private readonly IEmployeeService _employeeService;
        //constructor
        public ServerInformationControllers(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        //}
        /// <summary>
        /// Returns a server time as DateTime.UtcNow
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetServerTime")]
        [AllowAnonymousAttribute]
        public IActionResult GetServerTime()
        {
            try
            {
                return StatusCode(StatusCodes.Status200OK, LogInformation("ServerTime", _employeeService.GetServerTime()));

                //var serverTime = _employeeService.GetServerTime();
                //return Ok(serverTime);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, LogInformation("Error", $"{ex.Message}"));
            }
        }

        /// <summary>
        /// Returns a Day for given Date 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        [HttpGet("GetDay")]
        public IActionResult GetDay(DateTime? dateTime)
        {
            try
            {
                if(dateTime== null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, LogInformation("Error", $"Date is not provided"));
                }
                else
                {
                    var day = _employeeService.GetDay(dateTime);
                    return Ok(new Response { Status = "Success", Message = $"Given Date Day Is {day}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, LogInformation("Error", $"{ex.Message}"));
            }
        }

        private Response LogInformation(string status, string message)
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
