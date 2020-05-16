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
        public Guid ForgetPassword(string email);
        public bool ConfirmForgetting(Guid id, string email, string otp);
        public bool ChangePassword(Guid userId, string oldPassword, string newPassword);
    }
}
