namespace InternetBanking.Settings.Implementations
{
    public class AppSetting : ISetting
    {
        public string Temp { get; set; }
        public string RedisCacheEndpoint { get; set; }
        public string RedisCacheName { get; set; }
        public string ApplicationToken { get; set; }
        public string DBEndpoint { get; set; }
        public string DBName { get; set; }

        public string AesKey { get; set; }

        public string AesIv { get; set; }

        public uint Expiration { get; set; }
    }
}
