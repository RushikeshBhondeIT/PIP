using Entities;
using System;


namespace EmployeeServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService methods.
    /// </summary>
    public class CountryResponse
    {
        public Guid CountyId { get; set; }
        public string? CountryName { get; set; }

        /// <summary>
        /// It compare the current object to another object of CountryResponse type and return true,
        /// If both values are same return true otherwise false 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        { 
            if (obj == null) return false;
            //checking type
            if(obj.GetType() != typeof(CountryResponse)) return false;
            //convert obj to countryResponse type 
            CountryResponse country_to_compare=(CountryResponse)obj;
            //checking values of current object and Incoming object 
            return this.CountyId==country_to_compare.CountyId && 
                this.CountryName==country_to_compare.CountryName;
        }

        //to avoid warning  br
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class CountryExtesion
    {
        /// <summary>
        /// Convert from Country object to CountryResponse object
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountyId = country.CountryId,
                CountryName = country.CountryName,
            };
        }
    }
}
