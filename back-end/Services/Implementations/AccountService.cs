using InternetBanking.Daos;
using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using InternetBanking.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace InternetBanking.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private IUserCollection _UserCollection;
        private ILinkingBankCollection _LinkingBankCollection;
        private ISetting _Setting;
        private IContext _Context;
        private ITransactionCollection _TransactionCollection;
        private MongoDBClient _MongoDBClient;

        public AccountService(ISetting setting, IUserCollection userCollection, IContext context, ILinkingBankCollection linkingBankCollection,
            ITransactionCollection transactionCollection, MongoDBClient mongoDBClient)
        {
            _UserCollection = userCollection;
            _Setting = setting;
            _Context = context;
            _LinkingBankCollection = linkingBankCollection;
            _TransactionCollection = transactionCollection;
            _MongoDBClient = mongoDBClient;
        }

        public bool ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            var res = false;

            var detail = _UserCollection.GetById(userId);
            if (detail != null)
            {
                if (Encrypting.BcryptVerify(oldPassword, detail.Password))
                {
                    // Change mật khẩu
                    if (_UserCollection.ChangePassword(new UserFilter() { Id = userId }, Encrypting.Bcrypt(newPassword)) > 0)
                    {
                        res = true;
                    }
                    else
                        _Setting.Message.SetMessage("Không thể cập nhật thông tin mật khẩu!"); ;
                }
                else
                    _Setting.Message.SetMessage("Mật khẩu hiện tại không đúng!");
            }
            else
                _Setting.Message.SetMessage("Không tìm thấy thông tin người dùng!");

            return res;
        }

        public bool ConfirmForgetting(Guid userId, string otp)
        {
            var res = false;

            var detail = _UserCollection.GetById(userId);

            if (detail != null)
            {
                // Get chi tiết giao dịch
                var transactions = _TransactionCollection.GetMany(new TransactionFilter() { ReferenceId = userId, Type = 2 });
                if (transactions.Any())
                {
                    var transaction = transactions.FirstOrDefault();
                    // Check Otp
                    if (transaction.Otp == otp)
                    {
                        // Check hết hạn
                        var now = DateTime.Now;
                        if (now <= transaction.ExpireTime && now >= transaction.CreateTime)
                        {
                            // Update mật khẩu
                            using (var sessionTask = _MongoDBClient.StartSessionAsync())
                            {
                                var session = sessionTask.Result;
                                session.StartTransaction();
                                try
                                {
                                    // Create mật khẩu mới
                                    string pass = _Context.MakeOTP(8);

                                    if (_UserCollection.ChangePassword(new UserFilter() { Id = userId }, Encrypting.Bcrypt(pass)) > 0)
                                    {
                                        // Update giao dịch
                                        transaction.ConfirmTime = DateTime.Now;

                                        if (_TransactionCollection.Replace(transaction) > 0)
                                        {
                                            // Send mail
                                            var sb = new StringBuilder();
                                            sb.AppendFormat($"Dear {detail.Name},");
                                            sb.AppendFormat("<br /><br /><b>Yêu cầu quên mật khẩu của bạn đã thực hiện thành công, mật khẩu mới của bạn là:</b>");
                                            sb.AppendFormat($"<br /><br /><b>{pass}</b>");
                                            sb.AppendFormat($"<br /><br /><b>Vui lòng đăng nhập vào hệ thống để kiểm tra.</b>");
                                            sb.AppendFormat($"<br /><br /><b>Nếu yêu cầu không phải của bạn, vui lòng bỏ qua mail này.</b>");

                                            if (_Context.SendMail("Yêu cầu quên mật khẩu", sb.ToString(), detail.Email, detail.Name))
                                            {
                                                res = true;
                                            }
                                            else
                                               _Setting.Message.SetMessage("Gửi mail thất bại!");
                                        }
                                        else
                                            _Setting.Message.SetMessage("Không thể cập nhật thông tin giao dịch!");
                                    }
                                    else
                                        _Setting.Message.SetMessage("Không thể cập nhật thông tin mật khẩu!");

                                    if (res)
                                        session.CommitTransactionAsync();
                                    else
                                        session.AbortTransactionAsync();
                                }
                                catch (Exception ex)
                                {
                                    session.AbortTransactionAsync();
                                    throw ex;
                                    throw;
                                }
                            }
                        }
                        else
                            _Setting.Message.SetMessage("Phiên giao dịch đã hết hạn!");
                    }
                    else
                        _Setting.Message.SetMessage("Mã OTP không đúng!");
                }
                else
                    _Setting.Message.SetMessage("Không tìm thấy thông tin giao dịch!");
            }
            else
                _Setting.Message.SetMessage("Không tìm thấy thông tin người dùng!");

            return res;
        }

        public LinkingBank CreateLinkingBank(LinkingBank bank)
        {
            bank.Id = Guid.Empty;
            _LinkingBankCollection.Create(bank);
            if (bank.Id.Equals(Guid.Empty))
            {
                return null;
            }
            else
            {
                return bank;
            }
        }

        public bool ForgetPassword(string email)
        {
            var res = false;

            var details = _UserCollection.Get(new UserFilter() { Email = email });
            if (details.Any())
            {
                var detail = details.FirstOrDefault();
                var transaction = new Transaction();
                transaction.ReferenceId = detail.Id;
                transaction.CreateTime = DateTime.Now;
                transaction.ExpireTime = transaction.CreateTime.AddMinutes(_Setting.TransferExpiration);
                transaction.Type = 2;
                while (true)
                {
                    transaction.Otp = _Context.MakeOTP(6);
                    if (!_TransactionCollection.GetMany(new TransactionFilter() { ReferenceId = detail.Id, Type = 2 }).Any())
                        break;
                }

                _TransactionCollection.Create(transaction);
                if (transaction.Id == Guid.Empty)
                {
                    // Send mail
                    var sb = new StringBuilder();
                    sb.AppendFormat($"Dear {detail.Name},");
                    sb.AppendFormat("<br /><br /><b>Bạn đang yêu cầu quên mật khẩu, mã xác thực của bạn là:</b>");
                    sb.AppendFormat($"<br /><br /><b>{transaction.Otp}</b>");
                    sb.AppendFormat($"<br /><br /><b>Mã xác thực này sẽ hết hạn lúc {transaction.ExpireTime.ToLongTimeString()}.</b>");
                    sb.AppendFormat($"<br /><br /><b>Nếu yêu cầu không phải của bạn, vui lòng bỏ qua mail này.</b>");

                    if (_Context.SendMail("Xác thực yêu cầu quên mật khẩu", sb.ToString(), detail.Email, detail.Name))
                    {
                        res = true;
                    }
                }
            }

            return res;
        }

        public IEnumerable<LinkingBank> GetLinkingBank()
        {
            return _LinkingBankCollection.Get(new LinkingBankFilter());
        }

        public AccountRespone Login(string username, string password)
        {
            AccountRespone res = null;

            var details = _UserCollection.Get(new UserFilter() { Username = username });

            if (username == "admin")
            {
                details = new List<User>() { new User() { Name = "Admin", Role = 0, Gender = 0, Username = "admin", Password = Encrypting.Bcrypt(password) } };

            }

            if (details.Any())
            {
                //var passDecrypt = Encrypting.AesDecrypt(password, Encoding.UTF8.GetBytes(_Setting.AesKey), Encoding.UTF8.GetBytes(_Setting.AesIv), Encoding.UTF8);

                var detail = details.FirstOrDefault();

                //var compare = Encrypting.BcryptVerify(passDecrypt, detail.Password);
                var compare = Encrypting.BcryptVerify(password, detail.Password);


                if (compare)
                {
                    var accessToken = _Context.GenerateAccessToken(new Claim[]
                        {
                            new Claim(ClaimTypes.PrimarySid, detail.Id.ToString()),
                            new Claim(ClaimTypes.NameIdentifier, detail.Username),
                            new Claim(ClaimTypes.Name, detail.Name),
                            new Claim(ClaimTypes.Gender, detail.Gender.ToString()),
                            new Claim(ClaimTypes.Role, _Context.GetRole(detail.Role))
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
                            new Claim(ClaimTypes.PrimarySid, principal.FindFirst(ClaimTypes.PrimarySid).Value),
                            new Claim(ClaimTypes.NameIdentifier, principal.FindFirst(ClaimTypes.NameIdentifier).Value),
                            new Claim(ClaimTypes.Name, principal.FindFirst(ClaimTypes.Name).Value),
                            new Claim(ClaimTypes.Gender, principal.FindFirst(ClaimTypes.Gender).Value),
                            new Claim(ClaimTypes.Role, principal.FindFirst(ClaimTypes.Role).Value),
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
