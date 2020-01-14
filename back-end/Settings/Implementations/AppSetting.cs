namespace InternetBanking.Settings.Implementations
{
    public class AppSetting : ISetting
    {
        public string Temp { get; set; }
        public string RedisCacheEndpoint { get; set; }
        public string RedisCacheName { get; set; }
        public string ApplicationToken { get; set; }
    }
}
