using InternetBanking.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InternetBanking.Services
{
    public interface IAccountService
    {
        public AccountRespone Login(string username, string password);
        public void Logout(string token, long timeExpire);
        public void Logout(ClaimsPrincipal claimsPrincipal, HttpRequest httpRequest);
    }
}
