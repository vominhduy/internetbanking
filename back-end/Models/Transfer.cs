using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Models
{
    public class Transfer
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid InternalUserId { get; set; }
        public string AccountNumber { get; set; }
        public Guid LinkingBankId { get; set; }
        public decimal Money { get; set; }
        public bool IsSenderPay { get; set; }
        public bool IsInternal { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public string Otp { get; set; }
    }
}
