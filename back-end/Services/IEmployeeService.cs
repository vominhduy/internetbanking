using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System;
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
        public bool Delete(Guid id);
        public IEnumerable<HistoryTransaction> CrossCheckingIn(DateTime? From, DateTime? To, Guid? bankId);
        public IEnumerable<HistoryTransaction> CrossCheckingOut(DateTime? From, DateTime? To, Guid? bankId);
    }
}
