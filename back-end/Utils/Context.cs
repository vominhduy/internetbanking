using InternetBanking.Models;
using InternetBanking.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InternetBanking.Utils
{
    public interface IContext
    {
        void SetTokenBlackList(string token, long timeExpire);
        string GetRefreshToken(string refreshToken);
        void SetRefreshToken(string accessToken, string refreshToken);
        bool IsTokenBlackList(string token);
        string GetCurrentToken(HttpRequest httpRequest);
        string GetRole(byte roleType);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken();
        void SetTransfer(Guid userId, string code, Transfer transfer);
        Transfer GetTransfer(Guid userId, string code);
        public string MakeOTP(int length);
        public decimal TransactionCost(decimal money);
    }

    public class Context : IContext
    {
        private const string TOKENBLACKLIST = "TOKENBLACKLIST";
        private const string REFRESHTOKENLIST = "REFRESHTOKENLIST";
        private const string TRANSFER = "TRANSFER";
        private RedisCache _Cache;
        private ISetting _Setting;
        public Context(ISetting setting)
        {
            _Setting = setting;
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

        public void SetRefreshToken(string accessToken, string refreshToken)
        {
            _Cache.Set($"{REFRESHTOKENLIST}_{refreshToken}", accessToken, TimeSpan.FromMinutes(_Setting.RefreshTokenExpiration));
        }
        public string GetRefreshToken(string refreshToken)
        {
            var res = _Cache.Get<string>($"{REFRESHTOKENLIST}_{refreshToken}");
            return res;
        }

        public void SetTransfer(Guid userId, string code, Transfer transfer)
        {
            _Cache.Set($"{TRANSFER}_{userId.ToString()}_{code}", transfer, TimeSpan.FromMinutes(_Setting.TransferExpiration));
        }
        public Transfer GetTransfer(Guid userId, string code)
        {
            var res = _Cache.Get<Transfer>($"{TRANSFER}_{userId.ToString()}_{code}");
            return res;
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

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_Setting.ApplicationToken);
            var tokenDescriptorRefresh = new SecurityTokenDescriptor
            {
                Issuer = "Blinkingcaret",
                Audience = "Anyone",
                NotBefore = DateTime.UtcNow,
                Subject = new ClaimsIdentity(claims),
                // time expire
                Expires = DateTime.UtcNow.AddMinutes(_Setting.AccessTokenExpiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenRefresh = tokenHandler.CreateToken(tokenDescriptorRefresh);
            return tokenHandler.WriteToken(tokenRefresh);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Setting.ApplicationToken)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public string MakeOTP(int length)
        {
            string UpperCase = "QWERTYUIOPASDFGHJKLZXCVBNM";
            string LowerCase = "qwertyuiopasdfghjklzxcvbnm";
            string Digits = "1234567890";
            string allCharacters = UpperCase + LowerCase + Digits;
            //Random will give random charactors for given length  
            Random r = new Random();
            String password = "";
            for (int i = 0; i < length; i++)
            {
                double rand = r.NextDouble();
                if (i == 0)
                {
                    password += UpperCase.ToCharArray()[(int)Math.Floor(rand * UpperCase.Length)];
                }
                else
                {
                    password += allCharacters.ToCharArray()[(int)Math.Floor(rand * allCharacters.Length)];
                }
            }
            return password;
        }

        public decimal TransactionCost(decimal money)
        {
            return money/1000;
        }
    }
}
