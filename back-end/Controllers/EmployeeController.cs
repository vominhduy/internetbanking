using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    public class EmployeeController : ApiController
    {
        private ISetting _Setting;
        private IEmployeeService _Service;

        public EmployeeController(ISetting setting, IEmployeeService employeeService)
        {
            _Setting = setting;
            _Service = employeeService;
        }

        // GET: api/Employee/get
        [HttpGet("Get")]
        public IActionResult GetAll()
        {
            IEnumerable<Employee> records = _Service.GetEmployees(new EmployeeFilter() { Id = Guid.Empty, Name = "" });

            return Ok(records);
        }
    }
}