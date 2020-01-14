namespace InternetBanking.Settings
{
    public interface ISetting
    {
        public string Temp { get; }
        public string RedisCacheEndpoint { get; }
        public string RedisCacheName { get; }
        public string ApplicationToken { get; }
    }
}
