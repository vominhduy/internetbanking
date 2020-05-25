﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost("users")]
        public IActionResult GetUserId([FromBody] PayInfo info)
        {
            var res = _UserService.GetUsers(new Models.Filters.UserFilter() { AccountNumber = info.AccountNumber, Username = info.Username });
            if (res.Any())
                return Ok(res.FirstOrDefault());
            else
                return NotFound();
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
            var res = _Service.PayIn(UserId, payInfo);

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
        // [Authorize(Roles = "User")]
        public IActionResult HistoryIn(Guid userId)
        {
            var res = _UserService.HistoryIn(userId);

            //res = new List<TransactionHistory>() { 
            //    new TransactionHistory() { 
            //        AccountName = "Name",
            //        AccountNumber = "numberr",
            //        BankName = "bank",
            //        ConfirmTime = DateTime.Now,
            //         Description= "nhan tiền",
            //         Money = 231232
            //} 
            //};

            return Ok(res);
        }

        /// <summary>
        /// Giao dịch chuyển tiền
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/Out
        [HttpGet("Histories/{userId}/Out")]
        //[Authorize(Roles = "User")]
        public IActionResult HistoryOut(Guid userId)
        {
            var res = _UserService.HistoryOut(userId);

            //res = new List<TransactionHistory>() {
            //    new TransactionHistory() {
            //        AccountName = "Name",
            //        AccountNumber = "numberr",
            //        BankName = "bank",
            //        ConfirmTime = DateTime.Now,
            //         Description= "chuyển tiền",
            //         Money = 231232
            //}
            //};

            return Ok(res);
        }

        /// <summary>
        /// Giao dịch thanh toán nhắc nợ - được trả
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/Dept/In
        [HttpGet("Histories/{userId}/Dept/In")]
        //[Authorize(Roles = "User")]
        public IActionResult HistoryDeptIn(Guid userId)
        {
            var res = _UserService.HistoryDeptIn(userId);

            //res = new List<TransactionHistory>() {
            //    new TransactionHistory() {
            //        AccountName = "Name",
            //        AccountNumber = "numberr",
            //        BankName = "bank",
            //        ConfirmTime = DateTime.Now,
            //         Description= "nhắc nợ - được trả",
            //         Money = 231232
            //}
            //};

            return Ok(res);
        }

        /// <summary>
        /// Giao dịch thanh toán nhắc nợ - trả
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // POST: api/Employees/Histories/962c3538-65f9-40c3-98b4-0ce277c3f559/Dept/Out
        [HttpGet("Histories/{userId}/Dept/Out")]
       // [Authorize(Roles = "User")]
        public IActionResult HistoryDeptOut(Guid userId)
        {
            var res = _UserService.HistoryDeptOut(userId);

            //res = new List<TransactionHistory>() {
            //    new TransactionHistory() {
            //        AccountName = "Name",
            //        AccountNumber = "numberr",
            //        BankName = "bank",
            //        ConfirmTime = DateTime.Now,
            //         Description= "nhắc nợ - trả",
            //         Money = 231232
            //}
            //};

            return Ok(res);
        }
    }
}