using MongoDB.Bson.Serialization.Attributes;
using System;

namespace InternetBanking.Models
{
    public class Payee
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public Guid LinkingBankId { get; set; }
        public string MnemonicName { get; set; }
        [BsonIgnore]
        public User User { get; set; }
    }
}
