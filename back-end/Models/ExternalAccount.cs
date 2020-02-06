using MongoDB.Bson.Serialization.Attributes;
using System;

namespace InternetBanking.Models
{
    public class ExternalAccount
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
