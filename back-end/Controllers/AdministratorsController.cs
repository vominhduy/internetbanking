using System;
using System.Linq;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace InternetBanking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministratorsController : ApiController
    {
        private ISetting _Setting;
        private IEmployeeService _Service;
        private IUserService _UserService;

        public AdministratorsController(ISetting setting, IEmployeeService employeeService, IUserService userService)
        {
            _Setting = setting;
            _UserService = userService;
            _Service = employeeService;
        }

        // Quản lý danh sách nhân viên

        // Get tất cả
        // GET: api/Administrators/Employees
        [HttpGet("Employees")]
        public IActionResult GetAll()
        {
            var records = _Service.GetEmployees(new EmployeeFilter() { Id = Guid.Empty, Name = "" });

            return Ok(records);
        }

        // Get chi tiết nhân viên
        // GET: api/Administrators/Employees/962c3538-65f9-40c3-98b4-0ce277c3f559
        [HttpGet("Employees/{id}")]
        public IActionResult GetDetailEmployee([FromQuery] Guid id)
        {
            var records = _Service.GetEmployees(new EmployeeFilter() { Id = id, Name = "" });

            if (records.Any())
                return Ok(records.FirstOrDefault());
            else
                return NotFound();
        }

        // Update nhân viên
        // PUT: api/Administrators/Employees/962c3538-65f9-40c3-98b4-0ce277c3f559
        [HttpPut("Employees/{employeeId}")]
        public IActionResult UpdateEmployee([FromQuery] Guid employeeId, [FromBody] Employee employee)
        {
            employee.Id = employeeId;
            var res = _Service.Update(employee);

            return Ok(res);
        }

        // Create nhân viên
        // POST: api/Administrators/Employees
        [HttpPost("Employees")]
        public IActionResult AddEmployee([FromBody] Employee employee)
        {
            var res = _Service.Add(employee);

            return Ok(res);
        }

        // Delete nhân viên
        // DELETE: api/Administrators/Employees/962c3538-65f9-40c3-98b4-0ce277c3f559
        [HttpDelete("Employees/{employeeId}")]
        public IActionResult DeleteEmployee([FromQuery] Guid employeeId)
        {
            var res = _Service.Delete(employeeId);

            return Ok(res);
        }

        // Get đối soát - in
        // GET: api/Administrators/CrossCheckings/In
        [HttpGet("CrossCheckings/In")]
        public IActionResult GetCrossCheckingIn([FromBody] CrossChecking crossChecking)
        {
            var records = _Service.CrossCheckingIn(crossChecking.From, crossChecking.To, crossChecking.BankId);

            return Ok(records);
        }

        // Get đối soát - Out
        // GET: api/Administrators/CrossCheckings/Out
        [HttpGet("CrossCheckings/Out")]
        public IActionResult GetCrossCheckingOut([FromBody] CrossChecking crossChecking)
        {
            var records = _Service.CrossCheckingOut(crossChecking.From, crossChecking.To, crossChecking.BankId);

            return Ok(records);
        }
    }
}