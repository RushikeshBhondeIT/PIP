using EmployeeAPI.Models;
using LeapYearAPI.Models;

namespace LeapYearAPI.LeapYearRepository
{
    public interface ILeapYearRepository
    {
        List<LeapYearResponse> GetLeapYear(LeapYearRange leapYearRange);
        List<LeapYearDayResponse> GetLeapYearsDay(DateTime startDate, DateTime endDate);
    }
}
