using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace InternetBanking.Models
{
    public class Employee
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        [BsonElement]
        public List<Guid> GroupIds { get; set; } = new List<Guid>();
    }
}
