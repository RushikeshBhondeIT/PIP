using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using Entities;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EmployeeServices
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        public CountriesService()
        {
            _countries = new List<Country>();
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            // Validations
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            if(countryAddRequest.CountryName==null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
            }
            if(_countries.Where(country => country.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Given country name is alreday exist .");
            }
            //Convert object from CountryAddRequest to country type
            Country country = countryAddRequest.ToCoutry();

            //generate CountryID
            country.CountryId = Guid.NewGuid();
            //Add country object into _countries
            _countries.Add(country);
            return country.ToCountryResponse();

        }

        public List<CountryResponse> GetAllCountries()
        {
           return   _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountyId(Guid? countryId)
        {
            if (countryId == null) { return null; }
            Country? country_response_from_list = _countries.FirstOrDefault(country => country.CountryId == countryId);
            if (country_response_from_list == null) { return null; };
            return country_response_from_list.ToCountryResponse() ?? null;
        }

        public string GetServerTime(DateTime dateTime)
        {
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            DateTime dateValue = dateTime;
            //Display the DayOfWeek string representation
            var day = dateValue.DayOfWeek.ToString();
            Thread.CurrentThread.CurrentCulture = originalCulture;
            return day;
        }
    }
}