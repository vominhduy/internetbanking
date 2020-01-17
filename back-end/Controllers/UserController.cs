using System;
using System.Linq;
using System.Security.Claims;
using InternetBanking.Models;
using InternetBanking.Models.Constants;
using InternetBanking.Models.Filters;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        private ISetting _Setting;
        private IUserService _Service;

        public UserController(ISetting setting, IUserService service)
        {
            _Setting = setting;
            _Service = service;
        }

        // GET: api/User
        [HttpGet()]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var records = _Service.GetUsers(new UserFilter() { Id = Guid.Empty, Name = "" });

            return Ok(records);
        }

        // GET: api/User/31231123
        [HttpGet("{id}")]
        [Authorize(Roles = "User")]
        public IActionResult GetDetailUser([FromQuery] Guid id)
        {
            var records = _Service.GetUsers(new UserFilter() { Id = id, Name = "" });

            if (records.Any())
                return Ok(records.FirstOrDefault());
            else
                return NotFound();
        }

        // PUT: api/User
        [HttpPut()]
        [Authorize(Roles = "User")]
        public IActionResult Update([FromBody] User user)
        {
            var res = _Service.UpdateUser(user);

            return Ok(res);
        }

        // POST: api/User/SavingsAccount
        [HttpPost("SavingsAccount")]
        [Authorize(Roles = "User")]
        public IActionResult CreateSavingsAccount([FromBody] BankAccount bankAccount)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var res = _Service.AddSavingsAccount(new UserFilter() { Username = username }, bankAccount);

            return Ok(res);
        }

        // PUT: api/User/BankAccount?type=0
        [HttpPut("BankAccount")]
        [Authorize(Roles = "User")]
        public IActionResult UpdateBankAccount([FromQuery] BankAccountType type, [FromBody] BankAccount bankAccount)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
            var res = _Service.UpdateBankAccount(userId, type, bankAccount);

            return Ok(res);
        }

        // DELETE: api/User/SavingsAccount/24839489f79sd7
        [HttpDelete("SavingsAccount/{id}")]
        [Authorize(Roles = "User")]
        public IActionResult DeleteSavingsAccount([FromQuery] Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
            var res = _Service.DeleteSavingsAccount(userId, id);

            return Ok(res);
        }

        // PUT: api/User/Deposit
        [HttpPost("Deposit")]
        [Authorize(Roles = "User")]
        public IActionResult Deposit([FromBody] Deposit depositInfo)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
            var res = _Service.Deposit(userId, depositInfo.Type, depositInfo.Id, depositInfo.Money);

            return Ok(res);
        }
    }
}