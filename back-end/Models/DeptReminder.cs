using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace InternetBanking.Models
{
    public class DeptReminder
    {
        [BsonId]
        public Guid Id { get; set; }
        [JsonIgnore]
        public Guid RequestorId { get; set; }
        public string RequestorAccountNumber { get; set; }
        public string RecipientAccountNumber { get; set; }
        [JsonIgnore]
        public Guid RecipientId { get; set; }
        public string Description { get; set; }
        public decimal Money { get; set; }
        public bool IsPaid { get; set; }
        public bool IsCanceled { get; set; }
        public string CanceledNotes { get; set; }
    }
}
