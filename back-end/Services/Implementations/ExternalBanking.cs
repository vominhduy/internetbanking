using InternetBanking.Models.ViewModels;
using InternetBanking.Settings;
using InternetBanking.Utils;
using System;
using System.Collections.Generic;

namespace InternetBanking.Services.Implementations
{
    public interface IExternalBanking
    {
        ExternalInfoUserResponse GetInfoUser(string accountNumber);
        bool PayIn(string source, string dest, decimal amount, string message);
        void SetPartnerCode(string code);
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

            var info = CallAPIHelper.CallAPI<ExternalBankRes<ExternalInfoUserResponse>>(string.Concat(_url, "api/transactions/query_info"), "POST", obj, headers, addQueryParams: true);
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

            var info = CallAPIHelper.CallAPI<ExternalBankRes<ExternalTransferMoneyResponse>>(string.Concat(_url, "api/transactions/receive_external"), "POST", obj, headers, addQueryParams: true);

            if (info != null)
            {
                return info.data.is_success;
            }
            else
            {
                return false;
            }
        }

        public void SetPartnerCode(string code)
        {
            _partnerCode = "99793bb9137042a3a7f15950f1215950";
            _url = "http://bkt-banking.herokuapp.com/";
            _secretKey = "99793bb9137042a3a7f15950f1215950";
        }
    }
}
