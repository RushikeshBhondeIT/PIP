using EmployeeServiceContracts;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling.Internal;
using System.Globalization;
using Umbraco.Core;

namespace EmployeeAPI.Controllers
{
    [ApiController]
    public class ServerInformationControllers : Controller
    {
        /// <summary>
        /// Returns a server time as DateTime.UtcNow
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/v1/GetServerTime")]
        public IActionResult GetServerTime()
        {
            try
            {
                return Ok(new { servertime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) });
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
        public IActionResult GetDay(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
                DateTime? dateValue = dateTime;
                // Display the DayOfWeek string representation
                string? day = dateValue?.DayOfWeek.ToString();
                Thread.CurrentThread.CurrentCulture = originalCulture;
                return Ok($"Provided Date Day Is = " + day);
            }
            else { return BadRequest("DateTime not provided"); }

        }
    }
}
