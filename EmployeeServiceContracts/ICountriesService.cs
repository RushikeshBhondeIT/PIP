using EmployeeServiceContracts.DTO;
using Entities;

namespace EmployeeServiceContracts
{
    /// <summary>
    /// Interface Represents business logic for manipulating
    /// Country entity
    /// </summary>
    public interface ICountriesService
    {

        string GetServerTime(DateTime dateTime);

        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Returns the country object after adding it(including newly generated country id)</returns>
       CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all Countries from the list
        /// </summary>
        /// <returns> All countries from the list as CountryResponse</returns>
        List<CountryResponse> GetAllCountries();
        /// <summary>
        /// Returns a country object based on the given country id 
        /// </summary>
        /// <param name="id">CountryId (guid) to serch</param>
        /// <returns>Matching country as CountryResponse object</returns>
        CountryResponse? GetCountryByCountyId(Guid? countryId);
    }
}