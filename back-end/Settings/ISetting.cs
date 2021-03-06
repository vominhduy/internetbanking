﻿namespace InternetBanking.Settings
{
    public interface ISetting
    {
        public string Temp { get; }
        public string RedisCacheEndpoint { get; }
        public string RedisCacheName { get; }
        public string ApplicationToken { get; }
        public string DBEndpoint { get; }
        public string DBName { get; }
        public string AesKey { get; }
        public string AesIv { get; }
        public uint AccessTokenExpiration { get; }
        public uint RefreshTokenExpiration { get; }
        public uint TransferExpiration { get; }
        public IMessage Message { get; set; }
        public string BankCode { get; set; }
        public string MailEmail { get; set; }
        public string MailPassword { get; set; }
        public string MailName { get; set; }
        public int MailPort { get; set; }
        public string MailHost { get; set; }
        public bool MailIsSSL { get; set; }
    }
}
