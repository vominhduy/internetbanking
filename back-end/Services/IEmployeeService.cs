using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System.Collections.Generic;

namespace InternetBanking.Services
{
    public interface IEmployeeService
    {
        public IEnumerable<Employee> GetEmployees(EmployeeFilter employeeFilter);
        public bool Update(Employee employee);
        public Employee Add(Employee employee);
        public User AddUser(Account account);
        public bool PayIn(PayInfo payInfo);
    }
}
