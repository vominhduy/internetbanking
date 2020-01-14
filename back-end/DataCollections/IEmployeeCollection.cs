using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;

namespace InternetBanking.DataCollections
{
    public interface IEmployeeCollection
    {
        void Create(Employee employeeInfo);
        IEnumerable<Employee> Get(EmployeeFilter employeeFilter);
        void CreateTransaction(Employee employeeInfo);
        long Replace(Employee employeeInfo);
        long Update(Employee employeeInfo);
        long Delete(Guid id);
    }
}
