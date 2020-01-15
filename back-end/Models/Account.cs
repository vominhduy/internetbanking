using MongoDB.Bson.Serialization.Attributes;
using System;

namespace InternetBanking.Models
{
    public class Account
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        public byte Role { get; set; } // 0: Admin, 1: User, 2: Employee
    }
}
