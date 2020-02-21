using InternetBanking.Models;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="account">{ "Username": "", "Password": ""}</param>
        /// <returns>AccountRespone</returns>
        // POST: api/Accounts/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] RAccount account)
        {
            var record = _Service.Login(account.Username, account.Password);

            if (record != null)
                return Ok(record);
            else
                return Conflict("Wrong username or password!");
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns>bool</returns>
        // POST: api/Accounts/Logout
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            _Service.Logout(User, Request);
            return Ok();
        }

        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <returns>bool</returns>
        // POST: api/Accounts/Passwords/Forget
        [HttpPost("Passwords/Forget")]
        public IActionResult ForgetPassword([FromBody] Newtonsoft.Json.Linq.JObject email)
        {
            var res = _Service.ForgetPassword(email.Value<string>("Email"));
            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        /// <summary>
        /// Quên mật khẩu - confỉm
        /// </summary>
        /// <param name="otp"></param>
        /// <returns>bool</returns>
        // POST: api/Accounts/Passwords/ConfirmForgetting
        [HttpPost("Passwords/ConfirmForgetting")]
        public IActionResult ConfirmForgetting([FromBody] JObject otp)
        {
            var res = _Service.ConfirmForgetting(UserId, otp.Value<string>("Otp"));

            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="password">{ "OldPassword": "", "NewPassword": "" }</param>
        /// <returns>bool</returns>
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

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="dataToken">{ AccessToken: "", RefreshToken: "" }</param>
        /// <returns>AccountRespone</returns>
        // POST: api/Accounts/Tokens/Refresh
        [HttpPost("Tokens/Refresh")]
        [AllowAnonymous]
        public IActionResult Refresh([FromBody] AccountRespone dataToken)
        {
            var res = _Service.RefreshToken(dataToken.AccessToken, dataToken.RefreshToken);
            if (res != null)
                return Ok(res);
            else
                return Unauthorized();
        }

        /// <summary>
        /// Danh sách ngân hàng liên kết
        /// </summary>
        /// <returns>IEnumerable<LinkingBank></returns>
        // GET: api/Accounts/LinkingBanks
        [HttpGet("LinkingBanks")]
        public IActionResult GetLinkingBank()
        {
            var res = _Service.GetLinkingBank();
            return Ok(res);
        }
    }
}