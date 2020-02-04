using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace InternetBanking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiController : ControllerBase
    {
        protected Guid UserId
        {
            get
            {
                return Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
            }
        }
    }

    [Authorize]
    public abstract class AuthorizeApiController : ApiController
    {
    }

    /* Test running */
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new OkObjectResult(new string[] { "value1", "value2" });
        }
    }
    /* Test running */
}
