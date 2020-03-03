using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    public class UtilsController : ApiController
    {
        [HttpPost]
        public IActionResult CheckSum()
        {
            return Ok();
        }
    }
}