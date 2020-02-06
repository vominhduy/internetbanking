using System;
using System.Linq;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeesController : ApiController
    {
        private ISetting _Setting;
        private IEmployeeService _Service;
        private IUserService _UserService;

        public EmployeesController(ISetting setting, IEmployeeService employeeService, IUserService userService)
        {
            _Setting = setting;
            _UserService = userService;
            _Service = employeeService;
        }

        // GET: api/Employees
        [HttpGet()]
        public IActionResult GetAll()
        {
            var records = _Service.GetEmployees(new EmployeeFilter() { Id = Guid.Empty, Name = "" });

            return Ok(records);
        }

        // GET: api/Employees/31231123
        [HttpGet("{id}")]
        public IActionResult GetDetailEmployee([FromQuery] Guid id)
        {
            var records = _Service.GetEmployees(new EmployeeFilter() { Id = id, Name = "" });

            if (records.Any())
                return Ok(records.FirstOrDefault());
            else
                return NotFound();
        }

        // PUT: api/Employees
        [HttpPut()]
        public IActionResult UpdateEmployee([FromBody] Employee employee)
        {
            var res = _Service.Update(employee);

            return Ok(res);
        }
        // PUT: api/Employees
        [HttpPost()]
        public IActionResult AddEmployee([FromBody] Employee employee)
        {
            var res = _Service.Update(employee);

            return Ok(res);
        }

        // POST: api/Employees/Users
        [HttpPost()]
        public IActionResult AddUser([FromBody] Account user)
        {
            var res = _Service.AddUser(user);

            return Ok(res);
        }

        // POST: api/Employees/Users/PayIn
        [HttpPost("Users/PayIn")]
        public IActionResult PayIn([FromBody] PayInfo payInfo)
        {
            var res = _Service.PayIn(payInfo);

            return Ok(res);
        }

        // Xem lịch sử giao dịch của 1 tài khoản

        // Giao dịch nhận tiền
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/In
        [HttpGet("Histories/{userId}/In")]
        [Authorize(Roles = "User")]
        public IActionResult HistoryIn([FromQuery] Guid userId)
        {
            var res = _UserService.HistoryIn(userId);
            return Ok(res);
        }

        // Giao dịch chuyển tiền
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/Out
        [HttpGet("Histories/{userId}/Out")]
        [Authorize(Roles = "User")]
        public IActionResult HistoryOut([FromQuery] Guid userId)
        {
            var res = _UserService.HistoryOut(userId);
            return Ok(res);
        }

        // Giao dịch thanh toán nhắc nợ - được trả
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/Dept/In
        [HttpGet("Histories/{userId}/Dept/In")]
        [Authorize(Roles = "User")]
        public IActionResult HistoryDeptIn([FromQuery] Guid userId)
        {
            var res = _UserService.HistoryDeptIn(userId);
            return Ok(res);
        }
        // Giao dịch thanh toán nhắc nợ - trả
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/Dept/Out
        [HttpGet("Histories/{userId}/Dept/Out")]
        [Authorize(Roles = "User")]
        public IActionResult HistoryDeptOut([FromQuery] Guid userId)
        {
            var res = _UserService.HistoryDeptOut(userId);
            return Ok(res);
        }
    }
}