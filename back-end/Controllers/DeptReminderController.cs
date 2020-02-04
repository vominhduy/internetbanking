using System;
using InternetBanking.Models;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    [Authorize]
    public class DeptReminderController : ApiController
    {
        private ISetting _Setting;
        private IDeptReminderService _Service;

        public DeptReminderController(ISetting setting, IDeptReminderService service)
        {
            _Setting = setting;
            _Service = service;
        }

        #region Dept reminder
        // Get: api/Deptreminder
        [HttpGet()]
        [Authorize(Roles = "User")]
        public IActionResult GetDeptreminder()
        {
            var res = _Service.GetDeptReminders(UserId);

            return Ok(res);
        }

        // POST: api/Deptreminder
        [HttpPost()]
        [Authorize(Roles = "User")]
        public IActionResult AddDeptreminder([FromBody] DeptReminder deptReminder)
        {
            var res = _Service.AddDeptReminder(UserId, deptReminder);

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

        // POST: api/Deptreminder
        [HttpPost("Checkout/{id}")]
        [Authorize(Roles = "User")]
        public IActionResult CheckoutDeptreminder([FromQuery] Guid id)
        {
            var res = _Service.CheckoutDeptReminder(UserId, id);

            return Ok(res);
        }
        #endregion
    }
}