using InternetBanking.Models.ViewModels;
using InternetBanking.Settings;
using InternetBanking.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace InternetBanking.Services.Implementations
{
    public interface IExternalBanking
    {
        ExternalInfoUserResponse GetInfoUser(string accountNumber);
        bool PayIn(string source, string dest, decimal amount, string message);
        void SetPartnerCode();
    }

    /// <summary>
    ///  Bảo + Khuê 
    /// </summary>
    public class ExternalBanking_BKTBank : IExternalBanking
    {
        private readonly IEncrypt _encrypt;
        private readonly ISetting _setting;

        public ExternalBanking_BKTBank(IEncrypt encrypt, ISetting setting)
        {
            _encrypt = encrypt;
            _setting = setting;
        }

        public static string _url = "";
        public static string _secretKey = "";
        public static string _partnerCode = "";

        public ExternalInfoUserResponse GetInfoUser(string accountNumber)
        {
            var log = new StringBuilder();

            long timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            string hash = Encrypting.HMD5Hash($"{_partnerCode}|{timestamp.ToString()}|{accountNumber}", _secretKey);

            var headers = new Dictionary<string, string>()
            {
                {"partner_code", _partnerCode },
                { "timestamp", timestamp.ToString() },
                { "hash", hash }
            };

            var obj = new
            {
                account_number = accountNumber,
            };          
            var info = Helper.CallAPI<ExternalBankRes<ExternalInfoUserResponse>>(string.Concat(_url, "api/transactions/query_info"), "POST", obj, headers, addQueryParams: true);
            log.AppendLine(JsonConvert.SerializeObject(obj));
            log.AppendLine(JsonConvert.SerializeObject(info));
            LogTxt.WritetLog(log.ToString());
            if (info != null)
            {
                return info.data;
            }
            else
            {
                return null;
            }
        }

        public bool PayIn(string source, string dest, decimal amount, string message)
        {
            var log = new StringBuilder();
            long timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            string hash = Encrypting.HMD5Hash($"{_partnerCode}|{timestamp.ToString()}|{source}|{dest}|{(int)amount}|{message}", _secretKey);
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
                {"partner_code", _partnerCode },
                { "timestamp", timestamp.ToString() },
                { "hash", hash },
                {"signature", _encrypt.EncryptData(hash, _secretKey)}
            };

            var info = Helper.CallAPI<ExternalBankRes<ExternalTransferMoneyResponse>>(string.Concat(_url, "api/transactions/receive_external"), "POST", obj, headers, addQueryParams: true);
            log.AppendLine(JsonConvert.SerializeObject(obj));
            log.AppendLine(JsonConvert.SerializeObject(info));
            LogTxt.WritetLog(log.ToString());
            if (info != null)
            {
                return info.data.is_success;
            }
            else
            {
                return false;
            }
        }

        public void SetPartnerCode()
        {
            _partnerCode = "99793bb9137042a3a7f15950f1215950";
            _url = "http://bkt-banking.herokuapp.com/";
            _secretKey = "99793bb9137042a3a7f15950f1215950";
        }
    }

    /// <summary>
    /// 
    /// </summary>

    public class ExternalBanking_VuBank : IExternalBanking
    {
        private readonly IEncrypt _encrypt;
        private readonly ISetting _setting;

        public ExternalBanking_VuBank(IEncrypt encrypt, ISetting setting)
        {
            _encrypt = encrypt;
            _setting = setting;
        }

        public static string _url = "";
        public static string _secretKey = "";
        public static string _partnerCode = "";

        public ExternalInfoUserResponse GetInfoUser(string accountNumber)
        {
            var log = new StringBuilder();
            //accountNumber = "18424082";
            var result = new ExternalInfoUserResponse();
            long timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            //timestamp = 1590000027;
            string hash = Encrypting.MD5Hash($"{accountNumber}{timestamp}");

            var obj = new
            {
                account_number = accountNumber,
                timestamp = timestamp.ToString(),
                hash = hash
            };

            var info = Helper.CallAPI<ExternalBank_Vu>("http://118.69.190.28:5000/checkquota", "POST", obj);
            log.AppendLine(JsonConvert.SerializeObject(obj));
            log.AppendLine(JsonConvert.SerializeObject(info));
            LogTxt.WritetLog(log.ToString());
            if (info != null)
            {
                result.account_number = accountNumber;
                result.email = "No email";
                result.full_name = accountNumber;
                result.username = accountNumber;
                return result;
            }
            else
            {
                return null;
            }
        }

        public bool PayIn(string source, string dest, decimal amount, string message)
        {
            var log = new StringBuilder();
            long timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            string hash = Encrypting.MD5Hash($"{source}{timestamp}");
            _encrypt.SetKey(_setting.BankCode);

            var obj = new
            {
                moneytranfer = 1, //amount.ToString(),
                account_number = 1 ,//source,
                to_account_number = 2,//dest,
                hash = hash,
                timestamp = timestamp.ToString(),
                sign = _encrypt.EncryptData($"{timestamp}", _secretKey,2)
            };

            var info = Helper.CallAPI<ExternalBankPayIn_Vu>(string.Concat(_url, "/transmoney"), "POST", obj);
            log.AppendLine(JsonConvert.SerializeObject(obj));
            log.AppendLine(JsonConvert.SerializeObject(info));
            LogTxt.WritetLog(log.ToString());
            if (info != null)
            {

                return info != null;
            }
            else
            {
                return false;
            }
        }

        public void SetPartnerCode()
        {
            _partnerCode = "99793bb9137042a3a7f15950f1215950";
            _url = "http://118.69.190.28:5000";
            _secretKey = "99793bb9137042a3a7f15950f1215950";
        }
    }
}
