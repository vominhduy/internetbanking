using MongoDB.Bson.Serialization.Attributes;
using System;

namespace InternetBanking.Models
{
    public class Transaction
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid ReferenceId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public string Otp { get; set; }
    }
}
