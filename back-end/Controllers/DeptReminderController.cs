using System;
using System.Linq;
using System.Security.Claims;
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
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
            var res = _Service.GetDeptReminders(userId);

            return Ok(res);
        }

        // POST: api/Deptreminder
        [HttpPost()]
        [Authorize(Roles = "User")]
        public IActionResult AddDeptreminder([FromBody] DeptReminder deptReminder)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
            var res = _Service.AddDeptReminder(userId, deptReminder);

            return Ok(res);
        }

        // PUT: api/Deptreminder
        [HttpPut()]
        [Authorize(Roles = "User")]
        public IActionResult UpdateDeptreminder([FromBody] DeptReminder deptReminder)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
            var res = _Service.UpdateDeptReminder(userId, deptReminder);

            return Ok(res);
        }

        // DELETE: api/Deptreminder
        [HttpDelete("{id}")]
        [Authorize(Roles = "User")]
        public IActionResult DeleteDeptreminder([FromQuery] Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
            var res = _Service.DeleteDeptReminder(userId, id);

            return Ok(res);
        }

        // POST: api/Deptreminder
        [HttpPost("Checkout/{id}")]
        [Authorize(Roles = "User")]
        public IActionResult CheckoutDeptreminder([FromQuery] Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
            var res = _Service.CheckoutDeptReminder(userId, id);

            return Ok(res);
        }
        #endregion
    }
}