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
    public class PartnersController : ApiController
    {
        private ISetting _Setting;
        private IUserService _Service;

        public PartnersController(ISetting setting, IUserService service)
        {
            _Setting = setting;
            _Service = service;
        }

        // GET: api/Partners/31231123
        [HttpGet("{accountNumber}")]
        public IActionResult GetDetailUser([FromQuery] string accountNumber)
        {
            var record = _Service.GetDetailUserByPartner(accountNumber);

            if (record != null)
                return Ok(record);
            else
                return NotFound();
        }

        // GET: api/Partners/PayIn
        [HttpPost("PayIn")]
        public IActionResult PayIn([FromQuery] Transfer transfer)
        {
            var record = _Service.PayInByPartner(transfer);

            if (record)
                return Ok(record);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

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