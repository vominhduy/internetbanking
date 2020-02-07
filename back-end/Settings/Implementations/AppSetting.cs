namespace InternetBanking.Settings.Implementations
{
    public class AppSetting : ISetting
    {
        public AppSetting(IMessage message)
        {
            Message = message;
        }
        public string Temp { get; set; }
        public string RedisCacheEndpoint { get; set; }
        public string RedisCacheName { get; set; }
        public string ApplicationToken { get; set; }
        public string DBEndpoint { get; set; }
        public string DBName { get; set; }

        public string AesKey { get; set; }

        public string AesIv { get; set; }

        public uint AccessTokenExpiration { get; set; }
        public uint RefreshTokenExpiration { get; set; }
        public uint TransferExpiration { get; set; }
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
