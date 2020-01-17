using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace InternetBanking.Models
{
    public class DeptReminder
    {
        [BsonId]
        public Guid Id { get; set; }
        public Payee Requestor { get; set; }
        public Payee Recipient { get; set; }
        public string Desciption { get; set; }
        public decimal Money { get; set; }
        public bool IsPaid { get; set; }
    }
}
