using MongoDB.Bson.Serialization.Attributes;
using System;

namespace InternetBanking.Models
{
    public class PayInfo
    {
        public string UserName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Money { get; set; }
    }
}
