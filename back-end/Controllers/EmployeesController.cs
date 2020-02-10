using System;
using InternetBanking.Models;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    
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

        /// <summary>
        /// Tạo tài khoản khách hàng
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Account</returns>
        // POST: api/Employees/Users
        [HttpPost()]
        public IActionResult AddUser([FromBody] Account user)
        {
            var res = _Service.AddUser(user);

            return Ok(res);
        }

        /// <summary>
        /// Nộp tiền vào tài khoản khách hàng
        /// </summary>
        /// <param name="payInfo"></param>
        /// <returns>bool</returns>
        // POST: api/Employees/Users/PayIn
        [HttpPost("Users/PayIn")]
        public IActionResult PayIn([FromBody] PayInfo payInfo)
        {
            var res = _Service.PayIn(payInfo);

            return Ok(res);
        }

        // Xem lịch sử giao dịch của 1 tài khoản

        /// <summary>
        /// Giao dịch nhận tiền
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/In
        [HttpGet("Histories/{userId}/In")]
        [Authorize(Roles = "User")]
        public IActionResult HistoryIn([FromQuery] Guid userId)
        {
            var res = _UserService.HistoryIn(userId);
            return Ok(res);
        }

        /// <summary>
        /// Giao dịch chuyển tiền
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/Out
        [HttpGet("Histories/{userId}/Out")]
        [Authorize(Roles = "User")]
        public IActionResult HistoryOut([FromQuery] Guid userId)
        {
            var res = _UserService.HistoryOut(userId);
            return Ok(res);
        }

        /// <summary>
        /// Giao dịch thanh toán nhắc nợ - được trả
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/Dept/In
        [HttpGet("Histories/{userId}/Dept/In")]
        [Authorize(Roles = "User")]
        public IActionResult HistoryDeptIn([FromQuery] Guid userId)
        {
            var res = _UserService.HistoryDeptIn(userId);
            return Ok(res);
        }

        /// <summary>
        /// Giao dịch thanh toán nhắc nợ - trả
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable<TransactionHistory></returns>
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