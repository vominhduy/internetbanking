using InternetBanking.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace InternetBanking.Utils
{
    public interface IContext
    {
        void SetTokenBlackList(string token, long timeExpire);
        bool IsTokenBlackList(string token);
        string GetCurrentToken(HttpRequest httpRequest);
        string GetRole(byte roleType);
    }

    public class Context : IContext
    {
        private const string TOKENBLACKLIST = "TOKENBLACKLIST";
        private RedisCache _Cache;
        public Context(ISetting setting)
        {
            _Cache = new RedisCache(setting.RedisCacheEndpoint, setting.RedisCacheName);
        }

        private void Save<T>(string url, string key, T value)
        {
            _Cache.Set(key, value);
        }

        private T Get<T>(string key)
        {
            return _Cache.Get<T>(key);
        }

        public void SetTokenBlackList(string token, long timeExpire)
        {
            DateTime foo = DateTime.UtcNow;

            long currentTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            _Cache.Set($"{TOKENBLACKLIST}_{token}", timeExpire, TimeSpan.FromSeconds(timeExpire - currentTime));
        }

        public bool IsTokenBlackList(string token)
        {
            var res = _Cache.Get<long>($"{TOKENBLACKLIST}_{token}");
            return res != 0;
        }

        public string GetCurrentToken(HttpRequest httpRequest)
        {
            string tokenOld = null;
            StringValues authzHeaders;
            if (!httpRequest.Headers.TryGetValue("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
            }
            else
            {
                var bearerToken = authzHeaders.ElementAt(0);
                tokenOld = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            }
            return tokenOld;
        }

        public string GetRole(byte roleType)
        {
            string res = null;
            switch (roleType)
            {
                case 0:
                    res = "Admin";
                    break;
                case 1:
                    res = "User";
                    break;
                case 2:
                    res = "Employee";
                    break;
                default:
                    res = "User";
                    break;
            }
            return res;
        }
    }
}
