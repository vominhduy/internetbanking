using System;
using System.Linq;
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
            var res = _Service.AddSavingsAccount(new UserFilter() { Id = UserId }, bankAccount);

            return Ok(res);
        }

        // PUT: api/User/BankAccount?type=0
        [HttpPut("BankAccount")]
        [Authorize(Roles = "User")]
        public IActionResult UpdateBankAccount([FromQuery] BankAccountType type, [FromBody] BankAccount bankAccount)
        {
            var res = _Service.UpdateBankAccount(UserId, type, bankAccount);

            return Ok(res);
        }

        // DELETE: api/User/SavingsAccount/24839489f79sd7
        [HttpDelete("SavingsAccount/{id}")]
        [Authorize(Roles = "User")]
        public IActionResult DeleteSavingsAccount([FromQuery] Guid id)
        {
            var res = _Service.DeleteSavingsAccount(UserId, id);

            return Ok(res);
        }

        // PUT: api/User/Deposit
        [HttpPost("Deposit")]
        [Authorize(Roles = "User")]
        public IActionResult Deposit([FromBody] Deposit depositInfo)
        {
            var res = _Service.Deposit(UserId, depositInfo.Type, depositInfo.Id, depositInfo.Money);

            return Ok(res);
        }
        // Post: api/User/Payee
        [HttpPost("Payee")]
        [Authorize(Roles = "User")]
        public IActionResult AddPayee([FromBody] Payee payee)
        {
            var res = _Service.AddPayee(UserId, payee);

            return Ok(res);
        }

        // PUT: api/User/Payee
        [HttpPut("Payee")]
        [Authorize(Roles = "User")]
        public IActionResult UpdatePayee([FromBody] Payee payee)
        {
            var res = _Service.UpdatePayee(UserId, payee);

            return Ok(res);
        }

        // DELETE: api/User/Payee/2348ffgo834
        [HttpDelete("Payee/{id}")]
        [Authorize(Roles = "User")]
        public IActionResult DeletePayee([FromQuery] Guid id)
        {
            var res = _Service.DeletePayee(UserId, id);

            return Ok(res);
        }

        // POST: api/User/InternalTransfer
        [HttpPost("InternalTransfer")]
        [Authorize(Roles = "User")]
        public IActionResult InternalTransfer([FromBody] Transfer transfer)
        {
            transfer.IsInternal = true;
            var res = _Service.Transfer(UserId, transfer);

            if (res != null)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        // POST: api/User/ExternalTransfer
        [HttpPost("ExternalTransfer")]
        [Authorize(Roles = "User")]
        public IActionResult ExternalTransfer([FromBody] Transfer transfer)
        {
            transfer.IsInternal = false;
            var res = _Service.Transfer(UserId, transfer);

            if (res != null)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }
        // POST: api/User/ConfirmTransfer
        [HttpPost("ConfirmTransfer")]
        [Authorize(Roles = "User")]
        public IActionResult ConfirmTransfer([FromBody] Transfer transfer)
        {
            transfer.IsInternal = false;
            var res = _Service.ConfirmTransfer(UserId, transfer.Id, transfer.Otp);

            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }
    }
}