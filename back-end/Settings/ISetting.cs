namespace InternetBanking.Settings
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
        public IMessage Message { get; set; }
        public string BankCode { get; set; }
    }
}
