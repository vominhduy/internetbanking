using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using System.Collections.Generic;

namespace InternetBanking.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeCollection _EmployeeCollection;
        private ISetting _Setting;
        public EmployeeService(ISetting setting, IEmployeeCollection employeeCollection)
        {
            _EmployeeCollection = employeeCollection;
            _Setting = setting;
        }

        public Employee Add(Employee employee)
        {
            _EmployeeCollection.Create(employee);
            return employee;
        }

        public IEnumerable<Employee> GetEmployees(EmployeeFilter employeeFilter)
        {
            return _EmployeeCollection.Get(employeeFilter);
        }

        public bool Update(Employee employee)
        {
            return _EmployeeCollection.Replace(employee) >= 0;
        }
    }
}
