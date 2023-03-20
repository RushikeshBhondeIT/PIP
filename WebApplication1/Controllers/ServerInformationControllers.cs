using EmployeeAPI.Models;
using EmployeeServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;

namespace EmployeeAPI.Controllers
{
    [ApiController]
    [RoutePrefix("api/v1/")]
    [Authorize(Roles ="Admin,HR")]
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
        public IActionResult GetServerTime()
        {
            try
            {
               var serverTime= _employeeService.GetServerTime();
               return Ok(serverTime);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = $"{ex.Message}" });
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
                var day = _employeeService.GetDay(dateTime);
                return Ok( new Response { Status = "Success", Message = $"Given Date Day Is {day}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"{ex.Message}" });
            }
        }
    }
}
