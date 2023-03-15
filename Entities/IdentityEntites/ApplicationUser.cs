
using Microsoft.AspNetCore.Identity;
using System;


namespace Entities.IdentityEntites
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public string? EmployeeName { get; set; }    

    }
}
