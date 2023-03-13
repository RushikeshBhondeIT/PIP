using EmployeeServiceContracts;
using EmployeeServiceContracts.DTO;
using EmployeeServiceContracts.DTO.Enums;
using EmployeeServicesRepo.Heplers;
using Entities;


namespace EmployeeServicesRepo
{
    public class EmployeesServices : IEmployeeService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICountriesService _countries;

        public EmployeesServices(ApplicationDbContext employeeDbContext, ICountriesService countriesService)
        {
            _db = employeeDbContext;
            _countries = countriesService;
        }

        private EmployeeResponse ConvertEmployeeToEmployeeResponse(Employee employee)
        {
            EmployeeResponse employeeResponse = employee.ToEmployeeResponse();
            employeeResponse.CountryName = _countries.GetCountryByCountyId(employee.CountryID)
                ?.CountryName;
            return employeeResponse;
        }

        public EmployeeResponse AddEmployee(EmployeeAddRequest? empoyeeAddRequest)
        {
            if (empoyeeAddRequest == null) { throw new ArgumentNullException(); }
            ValidationHelper.ModelValidation(empoyeeAddRequest);
            Employee employee = empoyeeAddRequest.ToEmployee();
            employee.EmployeeId = Guid.NewGuid();
            _db.Employees.Add(employee);
            _db.SaveChanges();
            //NOTICE
            return (employee.ToEmployeeResponse());
        }

        public List<EmployeeResponse> GetAllEmployee()
        {
            return _db.Employees.ToList()
                .Select(s => s.ToEmployeeResponse()).ToList();
        }

        public EmployeeResponse? GetEmployeeById(Guid? employeeId)
        {
            if (employeeId == null) { return null; }
            Employee? employee = _db.Employees.FirstOrDefault(temp => temp.EmployeeId == employeeId);
            if (employee == null)
            {
                return null;
            }
            return employee.ToEmployeeResponse();
        }

        List<EmployeeResponse> IEmployeeService.GetFilteredEmployee(string serchBy, string? serchString)
        {
            List<EmployeeResponse> allEmployee = GetAllEmployee();
            List<EmployeeResponse> matchingEmployee = allEmployee;
            if (!string.IsNullOrWhiteSpace(serchBy) && !string.IsNullOrEmpty(serchString))
            {

                switch (serchBy)
                {
                    case nameof(Employee.EmployeeName):
                        matchingEmployee = allEmployee.Where(temp => (string.IsNullOrEmpty(temp.EmployeeName) ? temp.EmployeeName.Contains(serchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                        break;
                    //case nameof(Employee.Email):
                    //    matchingEmployee = allEmployee.Where(temp => (string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(serchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    //    break;
                    //case nameof(Employee.Gender):
                    //    matchingEmployee = allEmployee.Where(temp => (string.IsNullOrEmpty(temp.Gender.ToString()) ? temp.Gender.ToString().Contains(serchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    //    break;
                    //case nameof(Employee.CountryID):
                    //    matchingEmployee = allEmployee.Where(temp => (string.IsNullOrEmpty(temp.CountryName) ? temp.CountryName.Contains(serchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    //    break;
                    default:
                        matchingEmployee = allEmployee;
                        break;
                }

            }
            return matchingEmployee;
        }

        public List<EmployeeResponse> GetSoretedEmployee(List<EmployeeResponse> employees, string sortBy, SortOrderOption options)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return employees;
            }
            List<EmployeeResponse> sortedEmployeeS = (sortBy, options)
                switch
            {
                (nameof(EmployeeResponse.EmployeeName), SortOrderOption.ASC) =>
                employees.OrderBy(temp => temp.EmployeeName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(EmployeeResponse.EmployeeName), SortOrderOption.DSC) =>
              employees.OrderByDescending(temp => temp.EmployeeName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(EmployeeResponse.Email), SortOrderOption.ASC) =>
              employees.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(EmployeeResponse.Email), SortOrderOption.DSC) =>
               employees.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(EmployeeResponse.Gender), SortOrderOption.ASC) =>
               employees.OrderBy(temp => temp.Gender.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(EmployeeResponse.Gender), SortOrderOption.DSC) =>
               employees.OrderByDescending(temp => temp.Gender.ToString(), StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(EmployeeResponse.CountryName), SortOrderOption.ASC) =>
              employees.OrderBy(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(EmployeeResponse.CountryName), SortOrderOption.DSC) =>
               employees.OrderByDescending(temp => temp.CountryName, StringComparer.OrdinalIgnoreCase).ToList(),

                _ => employees

            };

            return sortedEmployeeS;
        }

        public EmployeeResponse UpdateEmployee(UpdateEmployeeRequest? updateEmployeeRequest)
        {
            if (updateEmployeeRequest == null)
            {
                throw new ArgumentNullException(nameof(Employee));
            }
            //validation
            ValidationHelper.ModelValidation(updateEmployeeRequest);
            //matching employee
            Employee? matchingEmployee = _db.Employees.FirstOrDefault(temp => temp.EmployeeId == updateEmployeeRequest.EmployeeId);
            if (matchingEmployee == null)
            {
                throw new ArgumentException("Given person id does not exist");
            }

            //update details
            matchingEmployee.EmployeeName = updateEmployeeRequest.EmployeeName;
            matchingEmployee.Email = updateEmployeeRequest.Email;
            matchingEmployee.DateOfBirth = updateEmployeeRequest.DateOfBirth;
            matchingEmployee.CountryID = updateEmployeeRequest.CountryID;
            matchingEmployee.Address = updateEmployeeRequest.Address;
            matchingEmployee.ReceiveNewsLetters = updateEmployeeRequest.ReceiveNewsLetters;
            _db.SaveChanges();

            return matchingEmployee.ToEmployeeResponse();
        }

        public bool DeleteEmployee(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            Employee? employee = _db.Employees.FirstOrDefault(temp => temp.EmployeeId == id);
            if (employee == null)
            {
                return false;
            }
            _db.Employees.Remove(_db.Employees.First(temp => temp.EmployeeId == id));
            _db.SaveChanges();
            return true;
        }
    }
}
