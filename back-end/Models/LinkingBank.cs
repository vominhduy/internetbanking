using InternetBanking.Models.Constants;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace InternetBanking.Models
{
    public class LinkingBank
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public LinkingBankType Type { get; set; }
    }
}
