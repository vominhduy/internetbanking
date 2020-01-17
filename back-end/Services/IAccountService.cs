using InternetBanking.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace InternetBanking.Services
{
    public interface IAccountService
    {
        public AccountRespone Login(string username, string password);
        public void Logout(string token, long timeExpire);
        public void Logout(ClaimsPrincipal claimsPrincipal, HttpRequest httpRequest);
        public AccountRespone RefreshToken(string accessToken, string refreshToken);
        public IEnumerable<LinkingBank> GetLinkingBank();
    }
}
