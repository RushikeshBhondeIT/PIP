
using Entities.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{/// <summary>
/// Person domain model
/// </summary>
    public class Employee
    {
        [Key]
        public Guid? EmployeeId{ get; set; }
        public string? EmployeeName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool? ReceiveNewsLetters { get; set; }
        //added
        public string? CountryName { get; set; }    


        [ForeignKey("CountryID")]
        public virtual Country? Country { get; set; }
      
    }
}
