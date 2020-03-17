using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternetBanking.Services;
using InternetBanking.Settings;
using InternetBanking.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InternetBanking.Controllers
{
    public class UtilsController : ApiController
    {
     
        private readonly IEncrypt _encrypt;
        public UtilsController(IEncrypt encrypt)
        {
            _encrypt = encrypt;
        }

        [HttpPost]
        public IActionResult MD5Hash(string input)
        {
            return Ok(Encrypting.MD5Hash(input));
        }

        [HttpGet]
        public IActionResult Sign(string partnerCode, string input)
        {
            _encrypt.SetKey(partnerCode);
            var signed = _encrypt.EncryptData(input);
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