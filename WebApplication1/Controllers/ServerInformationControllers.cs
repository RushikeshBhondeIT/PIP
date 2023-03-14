using EmployeeAPI.Models;
using EmployeeServiceContracts;
using Microsoft.AspNetCore.Mvc;


namespace EmployeeAPI.Controllers
{
    [ApiController]
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
        [HttpGet("api/v1/GetServerTime")]
        public IActionResult GetServerTime()
        {
            try
            {
                return Ok(_employeeService.GetServerTime());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }

        /// <summary>
        /// Returns a Day for given Date 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        [HttpGet("api/v1/GetDay")]
        public string GetDay( DateTime? dateTime)
        {
            try
            {
             return _employeeService.GetDay(dateTime);
               
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
    }
}
