using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Models.Request;
using InternetBanking.Models.ViewModels;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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
                    return Ok(new
                    {
                        code = 1,
                        message = "Successful",
                        data = new InfoUserResponse()
                        {
                            account_number = record.AccountNumber,
                            address = record.Address,
                            email = record.Email,
                            full_name = record.Name,
                            gender = record.Gender,
                            phone = record.Phone
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        code = -1,
                        message = "This bank account could not be found",
                        data = (string)null
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    code = -2,
                    message = "Exception: Please contact ddpbank for assistance",
                    data = (string)null
                });
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
                    SourceLinkingBankId = _bankService.GetLinkingBankById(new LinkingBankFilter() { Code = _Setting.BankCode}).Id, // Luôn lấy mặc định là chính ngân hàng của mình
                };
                var record = _Service.PayInByPartner(transferDao);

                if (record != Guid.Empty)
                {
                    return Ok(new
                    {
                        code = 1,
                        message = "Successful",
                        data = new
                        {
                            account_number = transfer.to_account_number,
                            money_transfer = transfer.amount,
                            transaction_id = record.ToString("N")
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        code = -1,
                        message = "Error in processing. Please contact ddpbank for assistance",
                        data = (string)null
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    code = -2,
                    message = "Exception: Please contact ddpbank for assistance",
                    data = (string)null
                });
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