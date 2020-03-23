using InternetBanking.Settings;
using InternetBanking.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Services.Implementations
{

    public interface IExternalBanking
    {
        dynamic GetInfoUser(string accountNumber);
        bool PayIn(string source, string dest, decimal amount, string message);
    }

    /// <summary>
    ///  Bảo + Khuê 
    /// </summary>
    public class ExternalBanking : IExternalBanking
    {
        private readonly IEncrypt _encrypt;
        private readonly ISetting _setting;

        public ExternalBanking(IEncrypt encrypt, ISetting setting)
        {
            _encrypt = encrypt;
            _setting = setting;
        }

        public static readonly string _url = "http://bkt-banking.herokuapp.com/";
        public static readonly string _secretKey = "99793bb9137042a3a7f15950f1215950";

        public dynamic GetInfoUser(string accountNumber)
        {
            string partnerCode = "bkt.partner";
            long timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            string hash = Encrypting.HMD5Hash($"{partnerCode}|{timestamp.ToString()}|{accountNumber}",_secretKey);

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
            string partnerCode = "bkt.partner";
            long timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            string hash = Encrypting.HMD5Hash($"{partnerCode}|{timestamp.ToString()}|{source}|{dest}|{(int)amount}|{message}", _secretKey);
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
                {"signature", _encrypt.EncryptData(hash)}
            };
  
            var info = CallAPIHelper.CallAPI<dynamic>(string.Concat(_url, "/api/transactions/receive_external"), "POST", obj, headers);

            return info;
        }
    }
}
