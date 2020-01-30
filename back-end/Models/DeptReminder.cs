using MongoDB.Bson.Serialization.Attributes;
using System;

namespace InternetBanking.Models
{
    public class DeptReminder
    {
        [BsonId]
        public Guid Id { get; set; }
        public string RequestorAccountNumber { get; set; }
        public string RecipientAccountNumber { get; set; }
        public string Desciption { get; set; }
        public decimal Money { get; set; }
        public bool IsPaid { get; set; }
    }
}
