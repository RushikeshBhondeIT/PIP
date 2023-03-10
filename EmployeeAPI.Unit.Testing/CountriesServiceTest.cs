using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using EmployeeServices;
using System;


namespace EmployeeAPI.Unit.Testing
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region AddCountry
        //When CountryAddRequest is null , it should thoe argument null exception.
        [Fact]
        public void AddCountry_NUllCountry()
        {
            CountryAddRequest? request = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                _countriesService.AddCountry(request);
            });
        }
        //When courtyName is null it should throw argumentNull Exception.

        [Fact]
        public void AddCountry_CountryNameIsNUll()
        {
            CountryAddRequest? request = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                _countriesService.AddCountry(request);
            });
        }
        //When CountryName is duplicate , it should throe ArgumentException.
        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "UAS" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "UAS" };

            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request1);
            });
        }
        //When you supply proper country name , i should insert the country name to the existing list of countries
        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "UAS" };
            //adedd country
            CountryResponse response = _countriesService.AddCountry(request1);
            //get all countries
            List<CountryResponse> countriesFromGetAllCountries =
                _countriesService.GetAllCountries();
            Assert.True(response.CountyId != Guid.Empty);
            //check added ones and Getactualonce from GetAllCountries.
           Assert.Contains(response, countriesFromGetAllCountries); 
        }
        #endregion

        #region GetAllCountries
        [Fact]
        public void GetAllCountries_IsEmptyList()
        {
            List<CountryResponse> countryResponses = _countriesService.GetAllCountries();
            Assert.Empty(countryResponses);
        }
        [Fact]
        public void GetAllCountryDetails_AddFewCountries()
        {
            List<CountryAddRequest> countryRequestList =
                new List<CountryAddRequest>() {
                    new CountryAddRequest() { CountryName="USA"},
                     new CountryAddRequest() { CountryName="INDIA"},
                      new CountryAddRequest() { CountryName=" RUSSIA"},
                };
            //add countries list in the countries_list_from_add_country
            List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

            foreach (CountryAddRequest country in countryRequestList)
            {
                //also add it in main list form _countriesService
                countries_list_from_add_country.Add(_countriesService.AddCountry(country));
            }
            //Get the actual added _countriesService list of CoutryResponse.
            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

            //check if the added and actual one is equal or not 
            foreach (CountryResponse expectedCountry in countries_list_from_add_country)
            {
                //override equals method.
                Assert.Contains(expectedCountry, actualCountryResponseList);
            }
        }
        #endregion

        #region GetCountryById
        [Fact]
        public void GetCountryByCountryId_NullCountryId()
        {
            Guid? countryID = null;
           CountryResponse? country_response_from_GetCountryByCountyId= _countriesService.GetCountryByCountyId(countryID);
            Assert.Null(country_response_from_GetCountryByCountyId);
        }
        [Fact]
        public void GetCountryByCountryId_ValidCountryId()
        {
            CountryAddRequest? country_add_request = new CountryAddRequest()
            {
                CountryName = "China"
            };
            CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);
            CountryResponse? country_response_from_get=_countriesService.GetCountryByCountyId(country_response_from_add.CountyId);
            Assert.Equal(country_response_from_add, country_response_from_get);
        }

        #endregion
    }
}
