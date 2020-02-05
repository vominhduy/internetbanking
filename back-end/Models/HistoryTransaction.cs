using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace InternetBanking.Models
{
    public class HistoryTransaction : Transaction
    {
        public decimal Money { get; set; }
        public string Description { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
    }
}
