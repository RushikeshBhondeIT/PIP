using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using Entities;
using System.Globalization;

namespace EmployeeServicesRepo
{
    public class CountriesService : ICountriesService
    {
        private readonly ApplicationDbContext _db;

        public CountriesService(ApplicationDbContext employeeDbContext)
        {
            _db = employeeDbContext;
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            try
            {
                if (countryAddRequest == null)
                {
                    throw new ArgumentNullException(nameof(countryAddRequest));
                }
                if (countryAddRequest.CountryName == null)
                {
                    throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
                }
                if (_db.Countries.Where(country => country.CountryName == countryAddRequest.CountryName).Count() > 0)
                {
                    throw new ArgumentException("Given country name is alreday exist .");
                }
                //Convert object from CountryAddRequest to country type
                Country country = countryAddRequest.ToCoutry();

                //generate CountryID
                country.CountryId = Guid.NewGuid();
                //Add country object into _db
                _db.Countries.Add(country);
                _db.SaveChanges();
                return country.ToCountryResponse();
            }
            catch
            {
                return new CountryResponse();
            }
            finally { _db.Dispose(); }

        }

        public List<CountryResponse> GetAllCountries()
        {
            try
            {
                return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { _db.Dispose(); }
        }

        public CountryResponse? GetCountryByCountyId(Guid? countryId)
        {
            try
            {
                if (countryId == null) { return null; }
                Country? country_response_from_list = _db.Countries.FirstOrDefault(country => country.CountryId == countryId);
                if (country_response_from_list == null) { return null; };
                return country_response_from_list.ToCountryResponse() ?? null;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { _db.Dispose(); }
        }

        public string GetServerTime(DateTime dateTime)
        {
            try
            {
                CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
                DateTime dateValue = dateTime;
                //Display the DayOfWeek string representation
                var day = dateValue.DayOfWeek.ToString();
                Thread.CurrentThread.CurrentCulture = originalCulture;
                return day;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}