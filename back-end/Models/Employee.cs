using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace InternetBanking.Models
{
    public class Employee : Account
    {
        [BsonElement]
        public List<Guid> GroupIds { get; set; } = new List<Guid>();
    }
}
