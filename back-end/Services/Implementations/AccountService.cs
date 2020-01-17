using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using InternetBanking.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace InternetBanking.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private IUserCollection _UserCollection;
        private ISetting _Setting;
        private IContext _Context;
        public AccountService(ISetting setting, IUserCollection userCollection, IContext context)
        {
            _UserCollection = userCollection;
            _Setting = setting;
            _Context = context;
        }
        public AccountRespone Login(string username, string password)
        {
            AccountRespone res = null;
            var details = _UserCollection.Get(new UserFilter() { Username = username });

            if (details.Any())
            {
                var passDecrypt = Encrypting.AesDecrypt(password, Encoding.UTF8.GetBytes(_Setting.AesKey), Encoding.UTF8.GetBytes(_Setting.AesIv), Encoding.UTF8);

                var detail = details.FirstOrDefault();

                var compare = Encrypting.MD5Verify(passDecrypt, detail.Password);

                if (compare)
                {
                    var accessToken = _Context.GenerateAccessToken(new Claim[]
                        {
                            new Claim(ClaimTypes.PrimarySid, detail.Id.ToString()),
                            new Claim(ClaimTypes.NameIdentifier, detail.Username),
                            new Claim(ClaimTypes.Name, detail.Name),
                            new Claim(ClaimTypes.Gender, detail.Gender.ToString())
                        });
                    var refreshToken = _Context.GenerateRefreshToken();

                    _Context.SetRefreshToken(accessToken, refreshToken);

                    res = new AccountRespone();
                    res.Name = detail.Name;
                    res.AccessToken = accessToken;
                    res.RefreshToken = refreshToken;
                }
            }
            return res;
        }

        public void Logout(string token, long timeExpire)
        {
            _Context.SetTokenBlackList(token, timeExpire);
        }

        public void Logout(ClaimsPrincipal claimsPrincipal, HttpRequest httpRequest)
        {
            var token = _Context.GetCurrentToken(httpRequest);
            var timeExpire = Convert.ToInt64(claimsPrincipal.FindFirst("exp").Value);

            _Context.SetTokenBlackList(token, timeExpire);
        }

        public AccountRespone RefreshToken(string accessToken, string refreshToken)
        {
            AccountRespone res = null;
            var oldAccessToken = _Context.GetRefreshToken(refreshToken);

            if (oldAccessToken == accessToken)
            {
                var principal = _Context.GetPrincipalFromExpiredToken(accessToken);

                res = new AccountRespone();

                accessToken = _Context.GenerateAccessToken(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, principal.FindFirst(ClaimTypes.NameIdentifier).Value),
                            new Claim(ClaimTypes.Name, principal.FindFirst(ClaimTypes.Name).Value),
                            new Claim(ClaimTypes.Gender, principal.FindFirst(ClaimTypes.Gender).Value)
                        });
                refreshToken = _Context.GenerateRefreshToken();

                _Context.SetRefreshToken(accessToken, refreshToken);


                res.Name = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                res.AccessToken = accessToken;
                res.RefreshToken = refreshToken;
            }
            return res;
        }
    }
}
