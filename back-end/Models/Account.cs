using MongoDB.Bson.Serialization.Attributes;
using System;

namespace InternetBanking.Models
{
    public class Account
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte Gender { get; set; } // 0: Nam; 1: Nu; 2: Khac
        public string Password { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        public byte Role { get; set; } // 0: Admin, 1: User, 2: Employee
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class RPassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class RAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
