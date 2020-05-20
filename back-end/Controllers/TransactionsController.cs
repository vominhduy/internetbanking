using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Models.Request;
using InternetBanking.Models.ViewModels;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace InternetBanking.Controllers
{
    /// <summary>
    /// Api liên kết ngân hàng
    /// </summary>
    //[Authorize]
    public class TransactionsController : ApiController
    {
        private ISetting _Setting;
        private IUserService _Service;
        private ILinkingBankService _bankService;

        public TransactionsController(ISetting setting, IUserService service, ILinkingBankService bankService)
        {
            _Setting = setting;
            _Service = service;
            _bankService = bankService;
        }

        /// <summary>
        /// Get thông tin tài khoản
        /// </summary>
        /// <param name="account_number"></param>
        /// <returns>ExternalAccount</returns>

        [HttpPost("query_info")]
        public IActionResult GetDetailUser([FromBody] InfoUserRequest info, [FromQuery] string partner_code, [FromQuery] string timestamp, [FromQuery] string hash)
        {
            try
            {
                var record = _Service.GetDetailUserByPartner(info.account_number);

                if (record != null)
                {
                    var t = new InfoUserRoot();
                    t.data = new InfoUserCustomResponse();
                    t.data.account_number = record.AccountNumber;
                    t.data.email = record.Email;
                    t.data.full_name = record.Name;
                    t.data.username = record.AccountNumber;
                    return Ok(t);
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "This bank account could not be found",
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Cộng tiền vào tài khoản
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns>bool</returns>
        [HttpPost("receive_external")]
        public IActionResult PayIn([FromBody] TransferMoneyRequest transfer, [FromQuery] string partner_code, [FromQuery] string timestamp, [FromQuery] string hash, [FromQuery] string signature)
        {
            try
            {
                var partnerCode = Request.Query["partner_code"];
                var signed = Request.Query["signature"];
                var transferDao = new Transfer()
                {
                    SignedData = signed,
                    SourceAccountNumber = transfer.from_account_number,
                    DestinationAccountNumber = transfer.to_account_number,
                    Money = transfer.amount,
                    Description = transfer.message,
                    DestinationLinkingBankId = _bankService.GetLinkingBankById(new LinkingBankFilter() { Code = partnerCode }).Id,
                    SourceLinkingBankId = _bankService.GetLinkingBankById(new LinkingBankFilter() { Code = _Setting.BankCode }).Id, // Luôn lấy mặc định là chính ngân hàng của mình
                };
                var record = _Service.PayInByPartner(transferDao);

                if (record != Guid.Empty)
                {
                    return Ok(new
                    {
                        result = "success"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        result = "Error in processing. Please contact ddpbank for assistance",
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Trừ tiền
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns>bool</returns>
        // POST: api/Partners/PayOut
        [HttpPost("PayOut")]
        public IActionResult PayOut([FromQuery] Transfer transfer)
        {
            var record = _Service.PayOutByPartner(transfer);

            if (record)
                return Ok(record);
            else
                return Conflict(_Setting.Message.GetMessage());
        }
    }
}