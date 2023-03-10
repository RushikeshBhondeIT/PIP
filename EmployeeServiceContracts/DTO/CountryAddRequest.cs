using Entities;
using System;


namespace EmployeeServiceContracts.DTO
{/// <summary>
/// DTO class for adding a new country
/// </summary>
    public class CountryAddRequest
    {
        public string? CountryName { get; set; } 
        
        //To make object to add the country
        public Country ToCoutry()
        {
            return new Country()
            {
                CountryName = CountryName
            };
        }
    }
}
