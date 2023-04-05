using EmployeeAPI.Models;
using LeapYearAPI.Models;


namespace LeapYearAPI.LeapYearRepository
{
    public interface ILeapYearRepository
    {
        List<int> GetLeapYear(LeapYearRange leapYearRange);
        List<LeapYearDayResponse> GetLeapYearsDay(DateTime startDate, DateTime endDate);
        LoginResponseModel LogInApiCall(LogInModel logIn);
    }
}
