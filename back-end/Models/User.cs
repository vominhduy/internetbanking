using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace InternetBanking.Models
{
    public class User : Account
    {
        public string AccountNumber { get; set; }
        public BankAccount CheckingAccount { get; set; } = new BankAccount();
        [BsonElement]
        public List<BankAccount> SavingsAccounts { get; set; } = new List<BankAccount>();
        [BsonElement]
        public List<Payee> Payees { get; set; } = new List<Payee>();
        [BsonElement]
        public List<Guid> SelfDeptReminderIds { get; set; } = new List<Guid>();
        [BsonElement]
        public List<Guid> OtherDeptReminderIds { get; set; } = new List<Guid>();
    }
}
