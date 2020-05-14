using System;
using System.Linq;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Services;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    //[Authorize(Roles = "Admin")]
    //[Route("api/[controller]")]
   // [ApiController]
    public class AdministratorsController : ApiController
    {
        private ISetting _Setting;
        private IEmployeeService _Service;
        private IUserService _UserService;
        private IAccountService _AccountService;

        public AdministratorsController(ISetting setting, IEmployeeService employeeService, IUserService userService, IAccountService accountService )
        {
            _Setting = setting;
            _UserService = userService;
            _Service = employeeService;
            _AccountService = accountService;
        }

        // Quản lý danh sách nhân viên
        /// <summary>
        /// Danh sách nhân viên
        /// </summary>
        /// <returns>IEnumerable<Employee></returns>
        // Get tất cả
        // GET: api/Administrators/Employees
        [HttpGet("Employees")]
        public IActionResult GetAll()
        {
            var records = _Service.GetEmployees(new UserFilter() { Id = Guid.Empty });

            return Ok(records);
        }

        /// <summary>
        /// Get chi tiết nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Employee</returns>
        // GET: api/Administrators/Employees/962c3538-65f9-40c3-98b4-0ce277c3f559
        [HttpGet("Employees/{id}")]
        public IActionResult GetDetailEmployee(Guid id)
        {
            var records = _Service.GetEmployees(new UserFilter() { Id = id });

            if (records.Any())
                return Ok(records.FirstOrDefault());
            else
                return NotFound();
        }

        /// <summary>
        /// Update nhân viên
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="employee"></param>
        /// <returns>bool</returns>
        // PUT: api/Administrators/Employees/962c3538-65f9-40c3-98b4-0ce277c3f559
        [HttpPut("Employees/{employeeId}")]
        public IActionResult UpdateEmployee(Guid employeeId, [FromBody] Employee employee)
        {
            employee.Id = employeeId;
            var res = _Service.Update(employee);

            return Ok(res);
        }

        /// <summary>
        /// Create nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns>Employee</returns>
        // POST: api/Administrators/Employees
        [HttpPost("Employees")]
        public IActionResult AddEmployee([FromBody] Employee employee)
        {
            var res = _Service.Add(employee);

            return Ok(res);
        }

        /// <summary>
        /// Delete nhân viên
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>bool</returns>
        // DELETE: api/Administrators/Employees/962c3538-65f9-40c3-98b4-0ce277c3f559
        [HttpDelete("Employees/{employeeId}")]
        public IActionResult DeleteEmployee(Guid employeeId)
        {
            var res = _Service.Delete(employeeId);

            return Ok(res);
        }

        /// <summary>
        /// Get đối soát - in
        /// </summary>
        /// <param name="crossChecking">{}</param>
        /// <returns></returns>
        // GET: api/Administrators/CrossCheckings/In
        [HttpGet("CrossCheckings/In")]
        public IActionResult GetCrossCheckingIn([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? bankId)
        {
            var records = _Service.CrossCheckingIn(from, to, bankId); 

            return Ok(records);
        }

        /// <summary>
        /// Get đối soát - Out
        /// </summary>
        /// <param name="crossChecking"></param>
        /// <returns></returns>
        // GET: api/Administrators/CrossCheckings/Out
        [HttpGet("CrossCheckings/Out")]
        public IActionResult GetCrossCheckingOut([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? bankId)
        {
            var records = _Service.CrossCheckingOut(from, to, bankId);

            return Ok(records);
        }
    }
}