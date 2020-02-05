using System;
using InternetBanking.Models;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace InternetBanking.Controllers
{
    [Authorize]
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

        // Xem danh sách nợ
        // Get: api/Deptreminders
        [HttpGet()]
        [Authorize(Roles = "User")]
        public IActionResult GetDeptreminder()
        {
            var res = _Service.GetDeptReminders(UserId);

            return Ok(res);
        }

        // Tạo nhắc nợ
        // POST: api/Deptreminders
        [HttpPost()]
        [Authorize(Roles = "User")]
        public IActionResult AddDeptreminder([FromBody] DeptReminder deptReminder)
        {
            var res = _Service.AddDeptReminder(UserId, deptReminder);

            return Ok(res);
        }

        // Hủy nhắc nợ
        // PUT: api/Deptreminders/00000000-0000-0000-0000-000000000000
        // { Notes: "" }
        [HttpPost("{id}")]
        [Authorize(Roles = "User")]
        public IActionResult Cancel([FromQuery] Guid id, [FromBody] JObject deptReminder)
        {
            var res = _Service.CancelDeptReminder(UserId, id, deptReminder.Value<string>("Notes"));

            return Ok(res);
        }

        // PUT: api/Deptreminder
        [HttpPut()]
        [Authorize(Roles = "User")]
        public IActionResult UpdateDeptreminder([FromBody] DeptReminder deptReminder)
        {
            var res = _Service.UpdateDeptReminder(UserId, deptReminder);

            return Ok(res);
        }

        // DELETE: api/Deptreminder
        [HttpDelete("{id}")]
        [Authorize(Roles = "User")]
        public IActionResult DeleteDeptreminder([FromQuery] Guid id)
        {
            var res = _Service.DeleteDeptReminder(UserId, id);

            return Ok(res);
        }

        // Thanh toán nhắc nợ
        // POST: api/Deptreminders/Checkout/dfduaf1731728378192389
        [HttpPost("Checkout/{id}")]
        [Authorize(Roles = "User")]
        public IActionResult CheckoutDeptreminder([FromQuery] Guid id)
        {
            var res = _Service.CheckoutDeptReminder(UserId, id);

            if (res != null)
                return Ok(res.Id);
            else
                return Conflict(_Setting.Message.GetMessage());
        }

        // Thanh toán nhắc nợ - confirm
        // POST: api/Deptreminders/Checkout/dfduaf1731728378192389
        [HttpPost("Cnfirm")]
        [Authorize(Roles = "User")]
        public IActionResult ConfirmDeptreminder([FromQuery] Transaction transaction)
        {
            var res = _Service.ConfirmDeptReminder(UserId, transaction.Id, transaction.Otp);

            if (res)
                return Ok(res);
            else
                return Conflict(_Setting.Message.GetMessage());
        }
        #endregion
    }
}