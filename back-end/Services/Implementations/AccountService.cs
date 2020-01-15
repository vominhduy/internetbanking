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
                    // add claim info
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_Setting.ApplicationToken);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, detail.Username),
                            new Claim(ClaimTypes.Name, detail.Name),
                            new Claim(ClaimTypes.Gender, detail.Gender.ToString())
                        }),
                        // time expire
                        Expires = DateTime.UtcNow.AddMinutes(_Setting.Expiration),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    

                    // get role of user
                    tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, _Context.GetRole(detail.Role)));

                    // create token
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);


                    var tokenDescriptorRefresh = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                       {
                            new Claim(ClaimTypes.NameIdentifier, tokenString)
                       }),
                        // time expire
                        Expires = DateTime.UtcNow.AddMinutes(_Setting.Expiration),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenRefresh = tokenHandler.CreateToken(tokenDescriptorRefresh);
                    var tokenStringRefresh = tokenHandler.WriteToken(tokenRefresh);

                    res.Name = detail.Name;
                    res.AccessToken = tokenString;
                    res.RefreshToken = tokenStringRefresh;
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
    }
}
