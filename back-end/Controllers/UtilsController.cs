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

            _external.SetPartnerCode();
           // var t1 =_external.GetInfoUser("0000000043");
            //var t2 =_external.PayIn("0170013731", "0000000043", 1000, "test");

            var t2 = _external.PayIn("1", "18424082", 1000, "test");

            _encrypt.SetKey(partnerCode);
        //    var signed = _encrypt.EncryptData(input, "",2);
            return Ok();
        }

        [HttpGet("Verify")]
        public IActionResult Verify(string partnerCode, string input )
        {
            var signed = @"-----BEGIN PGP MESSAGE-----
Version: DidiSoft OpenPGP Library for .NET 1.9.2

owEBbgKR/ZANAwACAdyA589KboLvAcsVdAh0ZXh0LnR4dF7MMKxtZXNzYWdliQJF
BAABAgAvBQJey848KBxsdGhkMjAyMEBnbWFpbC5jb20gPGx0aGQyMDIwQGdtYWls
LmNvbT4ACgkQ3IDnz0pugu+XEA/9F9LnBkJ0SPKYF6fc/ZdyehHrjoxHtu3fzpB3
x2C0IZqaS91AMkIL1YZZ+jPu5d+Gd11rJgTQfCqg6wfEITZBx5mBTzsjqoKQDGSJ
zSLEjiQbe2kFPVzpA6r/c+JlMdU9aZmt5Mz7qkJDY/WXnm7zLhxGb1O4g/vHzmcX
pbFAVzQaO/HFbWXWXcwqs3DsZtXpxSoGuo3Rfj4e/7z8jDqQcdhO9SVEMgBnrOh/
+1N5LKW7Jh/EpflsFG0HvWYvGXAHZVc/1IJrEsNEMcagjlIUeVGScAuaNR7p/CGq
OsT1shNvCqX3WvgpgSkbq35oiFXowcKkYZRBHrXQgD6/Pl7hPt/XTBws+iWk+ps4
pxFzU3qyLfys2hMRXO6BsKw/LOO5O3hixnlMvxqIH5RfaKTxoVUXkyv55yJrAdGX
PLraFqujsnWD9ikZghk8jZrzmmIDARrgpjTcr0ez9Z1FIbH9318yqQtXjUnmQkAg
pr656AiCcLShCMvCfJcr+hx/KolN4dz8wkMXPE6RRPhvdqgOKETjQmKNIPvKbDam
12VnkG9q699RfWFCycZG8yQ72zkS/rcvqcqhYZJTABYWHMQSjrGxEYLiv0tNznAK
vb7MHlIQTuwwAWZFHuMkLiSaNTpzCdRfqpfcFK5+3CpEqYtoxJC1STAUqI39gKDh
YR+fQ0c=
=nRXt
-----END PGP MESSAGE-----
";
            _encrypt.SetKey(partnerCode);
            var result = _encrypt.DecryptData(signed,input,2);
            return Ok(result);
        }
    }
}