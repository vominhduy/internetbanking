using System;
using System.Linq;
using InternetBanking.Models;
using InternetBanking.Models.Constants;
using InternetBanking.Models.Filters;
using InternetBanking.Services;
using InternetBanking.Services.Implementations;
using InternetBanking.Settings;
using InternetBanking.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    // [Authorize(Roles = "User")]
    public class UsersController : ApiController
    {
        private ISetting _Setting;
        private IUserService _Service;
        private IEncrypt _Encrypt;

        public UsersController(ISetting setting, IUserService service)
        {
            _Setting = setting;
            _Service = service;
        }

        //// GET: api/User
        //[HttpGet()]
        //[Authorize(Roles = "Admin")]
        //public IActionResult GetAll()
        //{
        //    var records = _Service.GetUsers(new UserFilter() { Id = Guid.Empty, Name = "" });

        //    return Ok(records);
        //}

        // GET: api/Users/31231123
        [HttpGet()]
        [AllowAnonymous]
        public IActionResult GetDetailUser()
        {
            var records = _Service.GetUsers(new UserFilter() { Id = this.UserId, Name = "" });

            if (records.Any())
                return Ok(records.FirstOrDefault());
            else
                return NotFound();
        }

        [HttpGet("infotransfer")]
        //[Authorize(Roles = "User")]
        public IActionResult GetInfoTransfer([FromQuery]string accountNumber, [FromQuery] Guid bankId)
        {
            if (bankId == Guid.Empty)
            {
                var records = _Service.GetUsers(new UserFilter() { AccountNumber = accountNumber });
                if (records.Any())
                    return Ok(records.Select(x => new
                    {
                        x.AccountNumber,
                        x.Name
                    }).FirstOrDefault());
                else
                    return NotFound();
            }
            else
            {
                IExternalBanking externalBanking = null;
                if (bankId == Guid.Parse("8df09f0a-fd6d-42b9-804c-575183dadaf3"))
                {
                    externalBanking = new ExternalBanking_BKTBank(_Encrypt, _Setting);
                    externalBanking.SetPartnerCode();
                }
                else if(bankId == Guid.Parse("a707ac8f-829f-5c41-8e35-30c58ee67a62"))
                {
                    externalBanking = new ExternalBanking_VuBank(_Encrypt, _Setting);
                    externalBanking.SetPartnerCode();
                }

                var result = externalBanking.GetInfoUser(accountNumber);
                if (result != null)
                {
                    return Ok(new
                    {
                        AccountNumber = result.account_number,
                        Name = result.full_name
                    });
                }
                else
                    return NotFound();
            }
        }
        //// PUT: api/User
        //[HttpPut()]
        //[Authorize(Roles = "User")]
        //public IActionResult Update([FromBody] User user)
        //{
        //    var res = _Service.UpdateUser(user);

        //    return Ok(res);
        //}

        /// <summary>
        /// Tạo tài khoản tiết kiệm
        /// </summary>
        /// <param name="bankAccount"></param>
        /// <returns>BankAccount</returns>
        // POST: api/Users/SavingsAccounts
        [HttpPost("SavingsAccounts")]
        //[Authorize(Roles = "User")]
        public IActionResult CreateSavingsAccount([FromBody] BankAccount bankAccount)
        {
            var res = _Service.AddSavingsAccount(new UserFilter() { Id = UserId }, bankAccount);

            return Ok(res);
        }

        /// <summary>
        /// Update tài khoản tiết kiệm / thanh toán
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bankAccount">{ "Id": 00000000-0000-0000-0000-000000000000, "Name": "", "Description": "" }</param>
        /// <returns>bool</returns>
        // PUT: api/Users/BankAccounts?type=0
        [HttpPut("BankAccounts")]
        //[Authorize(Roles = "User")]
        public IActionResult UpdateBankAccount([FromQuery] BankAccountType type, [FromBody] BankAccount bankAccount)
        {
            var res = _Service.UpdateBankAccount(UserId, type, bankAccount);

            return Ok(res);
        }

        /// <summary>
        /// Xóa tài khoản tiết kiệm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Users/SavingsAccounts/00000000-0000-0000-0000-000000000000
        [HttpDelete("SavingsAccounts/{id}")]
        //[Authorize(Roles = "User")]
        public IActionResult DeleteSavingsAccount(Guid id)
        {
            var res = _Service.DeleteSavingsAccount(UserId, id);

            return Ok(res);
        }

        //// PUT: api/User/Deposit
        //[HttpPost("Deposit")]
        //[Authorize(Roles = "User")]
        //public IActionResult Deposit([FromBody] Deposit depositInfo)
        //{
        //    var res = _Service.Deposit(UserId, depositInfo.Type, depositInfo.Id, depositInfo.Money);

        //    return Ok(res);
        //}

        /// <summary>
        /// Thêm thông tin người nhận
        /// </summary>
        /// <param name="payee"></param>
        /// <returns>Payee</returns>
        // Post: api/Users/Payees
        [HttpPost("Payees")]
        //[Authorize(Roles = "User")]
        public IActionResult AddPayee([FromBody] Payee payee)
        {
            var res = _Service.AddPayee(UserId, payee);

            return Ok(res);
        }

        /// <summary>
        /// Update thông tin người nhận
        /// </summary>
        /// <param name="payee"></param>
        /// <returns>bool</returns>
        // PUT: api/Users/Payees/00000000-0000-0000-0000-000000000000
        [HttpPut("Payees/{id}")]
        //[Authorize(Roles = "User")]
        public IActionResult UpdatePayee(Guid id, [FromBody] Payee payee)
        {
            payee.Id = id;
            var res = _Service.UpdatePayee(UserId, payee);

            return Ok(res);
        }

        /// <summary>
        /// Delete người nhận
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        // DELETE: api/Users/Payees/00000000-0000-0000-0000-000000000000
        [HttpPost("Payees/Delete/{id}")]
        //[Authorize(Roles = "User")]
        public IActionResult DeletePayee(Guid id)
        {
            var res = _Service.DeletePayee(UserId, id);

            return Ok(res);
        }

        /// <summary>
        /// Chuyển tiền nội bộ
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns>Guid</returns>
        // POST: api/Users/InternalTransfer
        [HttpPost("InternalTransfer")]
        //[Authorize(Roles = "User")]
        public IActionResult InternalTransfer([FromBody] Transfer transfer)
        {
            transfer.SaveRecepientLinkingBankId = transfer.DestinationLinkingBankId;
            transfer.DestinationLinkingBankId = Guid.Empty;
            var res = _Service.Transfer(UserId, transfer);

            if (res != null)
                return Ok(res.Id);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        /// <summary>
        /// Chuyển tiền liên ngân hàng
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns>Guid</returns>
        // POST: api/User/ExternalTransfer
        [HttpPost("ExternalTransfer")]
        //[Authorize(Roles = "User")]
        public IActionResult ExternalTransfer([FromBody] Transfer transfer)
        {
            if (transfer.DestinationLinkingBankId == Guid.Empty)
            {
                return BadRequest("Chưa chọn ngân hàng!");
            }

            if (string.IsNullOrEmpty(transfer.DestinationAccountNumber))
            {
                return BadRequest("Chưa chọn số tài khoản nhận!");
            }

            transfer.SaveRecepientLinkingBankId = transfer.DestinationLinkingBankId;
            var res = _Service.Transfer(UserId, transfer);

            if (res != null)
                return Ok(res.Id);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        /// <summary>
        /// Xác nhận chuyển tiền
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>bool</returns>
        // POST: api/Users/ConfirmTransfer/00000000-0000-0000-0000-000000000000?otp=123456
        [HttpPost("ConfirmTransfer/{id}")]
        //[Authorize(Roles = "User")]
        public IActionResult ConfirmTransfer(Guid id, [FromQuery] string otp)
        {
            var res = _Service.ConfirmTransfer(UserId, id, otp);

            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        // Xem lịch sử giao dịch của 1 tài khoản

        /// <summary>
        /// Danh sách giao dịch nhận tiền
        /// </summary>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // GET: api/Users/Histories/In
        [HttpGet("Histories/In")]
        //[Authorize(Roles = "User")]
        public IActionResult HistoryIn()
        {
            var res = _Service.HistoryIn(UserId);
            res = res.Where(x => x.ConfirmTime > DateTime.Now.AddDays(-30));
            return Ok(res);
        }

        /// <summary>
        /// Giao dịch chuyển tiền
        /// </summary>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // GET: api/Users/Histories/Out
        [HttpGet("Histories/Out")]
        //[Authorize(Roles = "User")]
        public IActionResult HistoryOut()
        {
            var res = _Service.HistoryOut(UserId);
            res = res.Where(x => x.ConfirmTime > DateTime.Now.AddDays(-30));
            return Ok(res);
        }

        /// <summary>
        /// Giao dịch thanh toán nhắc nợ - được trả
        /// </summary>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // POST: api/Users/Histories/Depts/In
        [HttpGet("Histories/Depts/In")]
        //[Authorize(Roles = "User")]
        public IActionResult HistoryDeptIn()
        {
            var res = _Service.HistoryDeptIn(UserId);
            res = res.Where(x => x.ConfirmTime > DateTime.Now.AddDays(-30));
            return Ok(res);
        }

        /// <summary>
        /// Giao dịch thanh toán nhắc nợ - trả
        /// </summary>
        /// <returns>IEnumerable<TransactionHistory></returns>
        // POST: api/Users/Histories/Depts/Out
        [HttpGet("Histories/Depts/Out")]
        //[Authorize(Roles = "User")]
        public IActionResult HistoryDeptOut()
        {
            var res = _Service.HistoryDeptOut(UserId);
            res = res.Where(x => x.ConfirmTime > DateTime.Now.AddDays(-30));
            return Ok(res);
        }

        [HttpPost("BankAccounts/{id}/close")]
        //[Authorize(Roles = "User")]
        public IActionResult CloseBankAccount(Guid id)
        {
            var res = _Service.CloseBankAccount(UserId, id);

            return Ok(res);
        }
    }
}