using InternetBanking.Models;
using Microsoft.AspNetCore.Http;
using System;
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
        public bool ForgetPassword(string email);
        public bool ConfirmForgetting(Guid userId, string otp);
        public bool ChangePassword(Guid userId, string oldPassword, string newPassword);
        public LinkingBank CreateLinkingBank(LinkingBank bank);
    }
}
