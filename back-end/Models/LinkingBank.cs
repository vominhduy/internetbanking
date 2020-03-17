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
        public string Code { get; set; }
        public LinkingBankType Type { get; set; }
        
        /// <summary>
        /// Dùng lưu thông tin cho pgp
        /// </summary>
        public string Password { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public int KeySize { get; set; }

        public LinkingBank()
        {
            KeySize = 1024;
        }
    }
}
