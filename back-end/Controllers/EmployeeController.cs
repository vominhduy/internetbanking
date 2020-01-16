using System;
using System.Linq;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Services;
using InternetBanking.Settings;
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

        // GET: api/Employee
        [HttpGet()]
        public IActionResult GetAll()
        {
            var records = _Service.GetEmployees(new EmployeeFilter() { Id = Guid.Empty, Name = "" });

            return Ok(records);
        }

        // GET: api/Employee/31231123
        [HttpGet("{id}")]
        public IActionResult GetDetailEmployee([FromQuery] Guid id)
        {
            var records = _Service.GetEmployees(new EmployeeFilter() { Id = id, Name = "" });

            if (records.Any())
                return Ok(records.FirstOrDefault());
            else
                return NotFound();
        }

        // PUT: api/Employee
        [HttpPut()]
        public IActionResult UpdateEmployee([FromBody] Employee employee)
        {
            var res = _Service.Update(employee);

            return Ok(res);
        }
        // PUT: api/Employee
        [HttpPost()]
        public IActionResult AddEmployee([FromBody] Employee employee)
        {
            var res = _Service.Update(employee);

            return Ok(res);
        }
    }
}