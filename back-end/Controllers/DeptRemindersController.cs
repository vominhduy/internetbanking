using System;
using InternetBanking.Models;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace InternetBanking.Controllers
{
    public class DeptRemindersController : ApiController
    {
        private ISetting _Setting;
        private IDeptReminderService _Service;

        public DeptRemindersController(ISetting setting, IDeptReminderService service)
        {
            _Setting = setting;
            _Service = service;
        }

        #region Dept reminder

        /// <summary>
        /// Xem danh sách nhắc nợ
        /// </summary>
        /// <returns>IEnumerable<DeptReminder></returns>
        // Get: api/Deptreminders
        [HttpGet()]
       // [Authorize(Roles = "User")]
        public IActionResult GetDeptreminder()
        {
            var res = _Service.GetDeptReminders(UserId);

            return Ok(res);
        }

        /// <summary>
        /// Tạo nhắc nợ
        /// </summary>
        /// <param name="deptReminder"></param>
        /// <returns>DeptReminder</returns>
        // POST: api/Deptreminders
        [HttpPost()]
        //[Authorize(Roles = "User")]
        public IActionResult AddDeptreminder([FromBody] DeptReminder deptReminder)
        {
            var res = _Service.AddDeptReminder(UserId, deptReminder);

            return Ok(res);
        }

        /// <summary>
        /// Hủy nhắc nợ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deptReminder">{ "Notes": "" }</param>
        /// <returns>bool</returns>
        // PUT: api/Deptreminders/00000000-0000-0000-0000-000000000000
        [HttpPost("{id}")]
        //[Authorize(Roles = "User")]
        public IActionResult Cancel([FromQuery] Guid id, [FromBody] JObject deptReminder)
        {
            var res = _Service.CancelDeptReminder(UserId, id, deptReminder.Value<string>("Notes"));

            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        //// PUT: api/Deptreminder
        //[HttpPut()]
        //[Authorize(Roles = "User")]
        //public IActionResult UpdateDeptreminder([FromBody] DeptReminder deptReminder)
        //{
        //    var res = _Service.UpdateDeptReminder(UserId, deptReminder);

        //    return Ok(res);
        //}

        //// DELETE: api/Deptreminder
        //[HttpDelete("{id}")]
        //[Authorize(Roles = "User")]
        //public IActionResult DeleteDeptreminder([FromQuery] Guid id)
        //{
        //    var res = _Service.DeleteDeptReminder(UserId, id);

        //    return Ok(res);
        //}

        /// <summary>
        /// Thanh toán nhắc nợ
        /// </summary>
        /// <param name="id">Id của nhắc nợ</param>
        /// <returns>Guid</returns>
        // POST: api/Deptreminders/Checkout/00000000-0000-0000-0000-000000000000
        [HttpPost("Checkout/{id}")]
        //[Authorize(Roles = "User")]
        public IActionResult CheckoutDeptreminder(Guid id)
        {
            var res = _Service.CheckoutDeptReminder(UserId, id);

            if (res != null)
                return Ok(res.Id);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        /// <summary>
        /// Thanh toán nhắc nợ - confirm
        /// </summary>
        /// <param name="id">Id của transaction được trả về từ api CheckoutDeptreminder</param>
        /// <param name="otp"></param>
        /// <returns></returns>
        // POST: api/Deptreminders/Confirm?id=00000000-0000-0000-0000-000000000000&otp=123456
        [HttpPost("Confirm")]
        //[Authorize(Roles = "User")]
        public IActionResult ConfirmDeptreminder([FromQuery] Guid id, [FromQuery] string otp)
        {
            var res = _Service.ConfirmDeptReminder(UserId, id, otp);

            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }
        #endregion
    }
}