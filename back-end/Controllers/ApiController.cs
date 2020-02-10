using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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
                if (User.Claims.Count() > 0)
                    return Guid.Parse(User.FindFirst(ClaimTypes.PrimarySid).Value);
                else
                    return Guid.Parse("00000000-0000-0000-0000-000000000001");
            }
        }
    }

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
