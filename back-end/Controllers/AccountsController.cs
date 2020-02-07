using InternetBanking.Models;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    public class AccountsController : ApiController
    {
        private ISetting _Setting;
        private IAccountService _Service;

        public AccountsController(ISetting setting, IAccountService service)
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

        // GET: api/Accounts/Passwords/Forget
        [HttpPost("Passwords/Forget")]
        public IActionResult ForgetPassword()
        {
            var res = _Service.ForgetPassword(UserId);
            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        // GET: api/Accounts/Passwords/ConfirmForgetting
        [HttpPost("Passwords/ConfirmForgetting/{otp}")]
        public IActionResult ConfirmForgetting([FromQuery] string otp)
        {
            var res = _Service.ConfirmForgetting(UserId, otp);

            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }


        // POST: api/Accounts/Passwords/Change
        [HttpPost("Passwords/Change")]
        public IActionResult ChangePassword([FromBody] RPassword password)
        {
            var res = _Service.ChangePassword(UserId, password.OldPassword, password.NewPassword);

            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
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

        // GET: api/Account/LinkingBank
        [HttpGet("LinkingBank")]
        public IActionResult GetLinkingBank()
        {
            var res = _Service.GetLinkingBank();
            return Ok(res);
        }
    }
}