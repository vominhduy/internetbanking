using InternetBanking.Settings;
using InternetBanking.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Services.Implementations
{
    /// <summary>
    ///  Bảo + Khuê 
    /// </summary>
    public class ExternalBanking
    {
        private readonly IEncrypt _encrypt;
        private readonly ISetting _setting;

        public ExternalBanking(IEncrypt encrypt, ISetting setting)
        {
            _encrypt = encrypt;
            _setting = setting;
        }

        public static readonly string _url = "http://bkt-banking.herokuapp.com/";

        public dynamic GetInfoUser(string accountNumber)
        {
            string partnerCode = "";
            long timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            string hash = Encrypting.MD5Hash($"{partnerCode}|{timestamp.ToString()}|{accountNumber}");

            var headers = new Dictionary<string, string>()
            {
                {"partner_code", partnerCode },
                { "timestamp", timestamp.ToString() },
                { "hash", hash }
            };

            var obj = new
            {
                account_number = accountNumber,
            };

            var info = CallAPIHelper.CallAPI<dynamic>(string.Concat(_url, "api/transactions/query_info"), "POST", obj, headers);

            return info;
        }

        public bool PayIn(string source, string dest, decimal amount, string message)
        {
            string partnerCode = "";
            long timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            string hash = Encrypting.MD5Hash($"{partnerCode}|{timestamp.ToString()}|{source}|{dest}|{(int)amount}|{message}");
            _encrypt.SetKey(_setting.BankCode);

            var obj = new
            {
                from_account_number = source,
                to_account_number = dest,
                amount = amount,
                message = message
            };

            var headers = new Dictionary<string, string>()
            {
                {"partner_code", partnerCode },
                { "timestamp", timestamp.ToString() },
                { "hash", hash },
                {"signature", _encrypt.EncryptData(JsonConvert.SerializeObject(obj))}
            };
  
            var info = CallAPIHelper.CallAPI<dynamic>(string.Concat(_url, "/api/transactions/receive_external"), "POST", obj, headers);

            return info;
        }
    }
}
