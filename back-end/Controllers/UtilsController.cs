using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternetBanking.Services;
using InternetBanking.Services.Implementations;
using InternetBanking.Settings;
using InternetBanking.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    public class UtilsController : ApiController
    {
     
        private readonly IEncrypt _encrypt;
        private readonly IExternalBanking  _external;

        public UtilsController(IEncrypt encrypt, IExternalBanking external)
        {
            _encrypt = encrypt;
            _external = external;
        }

        [HttpPost]
        public IActionResult MD5Hash(string input)
        {
            return Ok(Encrypting.MD5Hash(input));
        }

        [HttpGet]
        public IActionResult Sign(string partnerCode, string input)
        {

            _external.SetPartnerCode("a");
            var t1 =_external.GetInfoUser("0000000043");
            var t2 =_external.PayIn("0170013731", "0000000043", 1000, "test");

            _encrypt.SetKey(partnerCode);
            var signed = _encrypt.EncryptData(input, "");
            return Ok(signed);
        }

        [HttpGet("Verify")]
        public IActionResult Verify(string partnerCode, string input )
        {
           var signed =  Request.Headers["signed"];
            _encrypt.SetKey(partnerCode);
            var result = _encrypt.DecryptData(signed,input);
            return Ok(result);
        }
    }
}