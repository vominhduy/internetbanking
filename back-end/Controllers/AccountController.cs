using InternetBanking.Models;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    public class AccountController : ApiController
    {
        private ISetting _Setting;
        private IAccountService _Service;

        public AccountController(ISetting setting, IAccountService service)
        {
            _Setting = setting;
            _Service = service;
        }

        // GET: api/Employee
        [HttpPost("Login")]
        public IActionResult Login([FromBody] Account account)
        {
            var record = _Service.Login(account.Username, account.Password);

            if (record != null)
                return Ok(record);
            else
                return Conflict("Wrong username or password!");
        }

        // GET: api/Employee/31231123
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            _Service.Logout(User, Request);
            return Ok();
        }

        // POST: api/Account/31231123
        [HttpPost("Refresh")]
        public IActionResult Refresh([FromBody] AccountRespone dataToken)
        {
            var res = _Service.RefreshToken(dataToken.AccessToken, dataToken.RefreshToken);
            if (res != null)
                return Ok(res);
            else
                return Unauthorized();
        }
    }
}