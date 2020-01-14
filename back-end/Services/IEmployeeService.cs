﻿using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System.Collections.Generic;

namespace InternetBanking.Services
{
    public interface IEmployeeService
    {
        public IEnumerable<Employee> GetEmployees(EmployeeFilter employeeFilter);
    }
}
