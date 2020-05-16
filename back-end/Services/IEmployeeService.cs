using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;

namespace InternetBanking.Services
{
    public interface IEmployeeService
    {
        public IEnumerable<Employee> GetEmployees(UserFilter employeeFilter);
        public bool Update(Employee employee);
        public Employee Add(Employee employee);
        public User AddUser(Account account);
        public bool PayIn(Guid userId, PayInfo payInfo);
        public bool Delete(Guid id);
        public IEnumerable<CrossChecking> CrossCheckingIn(DateTime? from, DateTime? to, Guid? bankId);
        public IEnumerable<CrossChecking> CrossCheckingOut(DateTime? from, DateTime? to, Guid? bankId);
    }
}
