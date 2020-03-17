using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Models
{
    public class Transfer
    {
        [BsonId]
        public Guid Id { get; set; }
        public string SourceAccountNumber { get; set; }
        [JsonIgnore]
        public Guid SourceLinkingBankId { get; set; }
        public string DestinationAccountNumber { get; set; }
        public Guid DestinationLinkingBankId { get; set; }
        public decimal Money { get; set; }
        [JsonIgnore]
        public decimal Fee { get; set; }
        public bool IsSenderPay { get; set; }
        [JsonIgnore]
        public bool IsConfirmed { get; set; }
        public string Description { get; set; }
        public string SignedData { get; set; }
    }
}
