using InternetBanking.Models;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    /// <summary>
    /// Api liên kết ngân hàng
    /// </summary>
    //[Authorize]
    public class PartnersController : ApiController
    {
        private ISetting _Setting;
        private IUserService _Service;

        public PartnersController(ISetting setting, IUserService service)
        {
            _Setting = setting;
            _Service = service;
        }

        /// <summary>
        /// Get thông tin tài khoản
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns>ExternalAccount</returns>
        // GET: api/Partners/31231123
        [HttpGet("{accountNumber}")]
        public IActionResult GetDetailUser(string accountNumber)
        {
            var record = _Service.GetDetailUserByPartner(accountNumber);

            if (record != null)
            {
                return Ok(new
                {
                    code = 1,
                    message = "Successful",
                    data = record
                });
            }
            else
            {
                return Ok(new
                {
                    code = -1,
                    message = "Fail",
                    data = (string)null
                });
            }
        }

        /// <summary>
        /// Cộng tiền vào tài khoản
        /// </summary>
        /// <param name="transfer"></param>
        /// <returns>bool</returns>
        // GET: api/Partners/PayIn
        [HttpPost("PayIn")]
        public IActionResult PayIn([FromBody] Transfer transfer)
        {
            var record = _Service.PayInByPartner(transfer);

            if (record)
            {
                return Ok(new
                {
                    code = 1,
                    message = "Successful",
                    data = new
                    {
                        account_number = transfer.DestinationAccountNumber,
                        money_transfer = transfer.Money
                    }
                });
            }   
            else
            {
                return Ok(new
                {
                    code = -1,
                    message = _Setting.Message.GetMessage() ?? "Fail",
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