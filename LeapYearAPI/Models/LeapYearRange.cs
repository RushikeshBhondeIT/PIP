
using System.ComponentModel.DataAnnotations;

namespace EmployeeAPI.Models
{
    public class LeapYearRange
    {
        [Required(ErrorMessage ="Start year cant be null")]
        public int StartYear { get; set; }
        [Required(ErrorMessage = "End year cant be null")]
        public int EndYear { get; set; }
    }
}
