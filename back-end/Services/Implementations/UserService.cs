using InternetBanking.Daos;
using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Constants;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using InternetBanking.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternetBanking.Services.Implementations
{
    public class UserService : IUserService
    {
        private IUserCollection _UserCollection;
        private ILinkingBankCollection _LinkingBankCollection;
        private ISetting _Setting;
        private IContext _Context;
        private ITransferCollection _TransferCollection;
        private MongoDBClient _MongoDBClient;
        public UserService(ISetting setting, IUserCollection userCollection, ILinkingBankCollection linkingBankCollection, IContext context
            , ITransferCollection transferCollection, MongoDBClient mongoDBClient)
        {
            _UserCollection = userCollection;
            _Setting = setting;
            _LinkingBankCollection = linkingBankCollection;
            _Context = context;
            _TransferCollection = transferCollection;
            _MongoDBClient = mongoDBClient;
        }

        public Payee AddPayee(Guid userId, Payee payee)
        {
            Payee res = null;
            var detail = _UserCollection.GetById(userId);

            if (detail != null)
            {
                var linkingBank = _LinkingBankCollection.GetById(payee.LinkingBankId);

                if (linkingBank != null)
                {
                    var countModified = _UserCollection.AddPayee(userId, payee);
                    if (countModified > 0)
                    {
                        res = payee;
                    }
                }
            }

            return res;
        }

        public BankAccount AddSavingsAccount(UserFilter userFilter, BankAccount bankAccount)
        {
            BankAccount res = null;
            var details = _UserCollection.Get(userFilter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault();

                if (detail.SavingsAccounts.FirstOrDefault(x => x.Name == bankAccount.Name) != null)
                {
                    _Setting.Message.SetMessage("Duplicate name of bank account!");
                }
                else
                {
                    bankAccount.Id = Guid.NewGuid();
                    userFilter.Name = null;
                    userFilter.Id = detail.Id;
                    var countModified = _UserCollection.AddSavingsAccount(userFilter, bankAccount);
                    if (countModified > 0)
                        res = bankAccount;
                }
            }

            return bankAccount;
        }

        public User AddUser(User user)
        {
            _UserCollection.Create(user);
            return user;
        }

        public bool ConfirmTransfer(Guid userId, Guid transferId, string otp)
        {
            var res = false;
            using (var sessionTask = _MongoDBClient.StartSessionAsync())
            {
                var session = sessionTask.Result;
                session.StartTransaction();
                try
                {
                    // Get detail user
                    var userDetail = _UserCollection.GetById(userId);

                    if (userDetail != null)
                    {
                        // Get transfer detail
                        var transfers = _TransferCollection.GetMany(new TransferFilter() { InternalUserId = userId, Id = transferId });
                        if (transfers.Any())
                        {
                            var transfer = transfers.FirstOrDefault();

                            // Check OTP
                            if (transfer.Otp == otp)
                            {
                                // Kiem tra het han
                                if (transfer.ExpireTime >= DateTime.Now)
                                {
                                    // Tru so du
                                    userDetail.CheckingAccount.AccountBalance -= transfer.Money;
                                    var fee = _Context.TransactionCost(transfer.Money);

                                    // Tru phi
                                    if (transfer.IsSenderPay)
                                    {
                                        userDetail.CheckingAccount.AccountBalance -= fee;
                                    }

                                    if (userDetail.CheckingAccount.AccountBalance >= 0)
                                    {
                                        // Luu thong tin nguoi gui
                                        var payOut = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = userId }, userDetail.CheckingAccount);

                                        if (payOut > 0)
                                        {
                                            // Cong tien nguoi nhan
                                            var success = false;
                                            if (transfer.IsInternal)
                                            {
                                                // noi bo
                                                var detailRecepients = _UserCollection.Get(new UserFilter() { AccountNumber = transfer.AccountNumber });
                                                if (detailRecepients.Any())
                                                {
                                                    var detailRecepient = detailRecepients.FirstOrDefault();
                                                    detailRecepient.CheckingAccount.AccountBalance += transfer.Money;
                                                    // Tru phi
                                                    if (!transfer.IsSenderPay)
                                                    {
                                                        detailRecepient.CheckingAccount.AccountBalance -= fee;
                                                    }

                                                    payOut = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = detailRecepient.Id }, detailRecepient.CheckingAccount);

                                                    if (payOut > 0)
                                                    {
                                                        success = true;
                                                    }
                                                }
                                                else
                                                {
                                                    _Setting.Message.SetMessage("Không tìm thấy thông tin người nhận!");
                                                }
                                            }
                                            else
                                            {
                                                // lien ngan hang
                                                // TODO
                                            }

                                            if (success)
                                            {
                                                // Update trang thai giao dich
                                                transfer.ConfirmTime = DateTime.Now;
                                                var updateTransfer = _TransferCollection.Replace(transfer);
                                                if (updateTransfer > 0)
                                                {
                                                    // Send mail
                                                    // TODO
                                                    res = true;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _Setting.Message.SetMessage("Số dư không đủ!");
                                    }
                                }
                                else
                                {
                                    _Setting.Message.SetMessage("Phiên giao dịch hết hạn!");
                                }
                            }
                            else
                            {
                                _Setting.Message.SetMessage("Sai mã OTP!");
                            }
                        }
                        else
                        {
                            _Setting.Message.SetMessage("Không tìm thấy thông tin giao dịch!");
                        }
                    }
                    else
                    {
                        _Setting.Message.SetMessage("Không tìm thấy thông tin người gửi yêu cầu!");
                    }

                    if (res)
                    {
                        session.CommitTransactionAsync().Wait();
                    }
                    else
                    {
                        session.AbortTransactionAsync().Wait();
                    }
                }
                catch (Exception)
                {
                    session.AbortTransactionAsync().Wait();
                    throw;
                }
            }

            return res;
        }

        public bool DeletePayee(Guid userId, Guid payeeId)
        {
            var res = false;
            var filter = new UserFilter();
            filter.Id = userId;
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault();
                var payee = detail.Payees.FirstOrDefault(x => x.Id == payeeId);

                if (payee != null)
                {
                    detail.Payees.Remove(payee);
                    res = _UserCollection.Replace(detail) >= 0;
                }
            }

            return res;
        }

        public bool DeleteSavingsAccount(Guid userId, Guid bankAccountId)
        {
            var res = false;
            var filter = new UserFilter();
            filter.Id = userId;
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault();
                var bankAccount = detail.SavingsAccounts.FirstOrDefault(x => x.Id == bankAccountId);

                if (bankAccount != null)
                {
                    detail.SavingsAccounts.Remove(bankAccount);
                    res = _UserCollection.Replace(detail) >= 0;
                }
            }

            return res;
        }

        public bool Deposit(Guid userId, BankAccountType bankAccountType, Guid bankAccountId, decimal money)
        {
            var res = false;
            var filter = new UserFilter();
            filter.Id = userId;
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault();
                if (bankAccountType == BankAccountType.CheckingAccount)
                {
                    detail.CheckingAccount.AccountBalance += money;
                    res = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = userId }, detail.CheckingAccount) >= 0;
                }
                else
                {
                    var bankDetail = detail.SavingsAccounts.FirstOrDefault(x => x.Id == bankAccountId);

                    if (bankDetail != null)
                    {
                        bankDetail.AccountBalance += money;
                        res = _UserCollection.UpdateSavingsAccount(new UserFilter() { Id = userId }, bankDetail) >= 0;
                    }
                }
            }

            return res;
        }

        public IEnumerable<User> GetUsers(UserFilter employeeFilter)
        {
            return _UserCollection.Get(employeeFilter);
        }

        public Transfer Transfer(Guid userId, Transfer transfer)
        {
            Transfer res = null;

            var userDetail = _UserCollection.GetById(userId);
            transfer.InternalUserId = userId;

            if (userDetail != null)
            {
                User recepient = null;

                if (transfer.IsInternal)
                {
                    var recepients = _UserCollection.Get(new UserFilter() { AccountNumber = transfer.AccountNumber });
                    if (recepients.Any())
                        recepient = recepients.FirstOrDefault();
                }
                else
                {
                    // Get recepient info from linking bank
                    // TODO
                }

                if (recepient != null)
                {
                    // Kiem tra so du
                    if (transfer.IsSenderPay && transfer.Money + _Context.TransactionCost(transfer.Money) > userDetail.CheckingAccount.AccountBalance)
                    {
                        _Setting.Message.SetMessage("Tài khoản không đủ số dư!");
                    }
                    else
                    {
                        string otp = null;
                        using (var sessionTask = _MongoDBClient.StartSessionAsync())
                        {
                            var session = sessionTask.Result;
                            session.StartTransaction();
                            try
                            {

                                // Create OTP
                                while (true)
                                {
                                    otp = _Context.MakeOTP(6);
                                    if (!_TransferCollection.GetMany(new TransferFilter() { InternalUserId = userId, Otp = otp }).Any())
                                        break;
                                }

                                // expire
                                transfer.CreateTime = DateTime.Now;
                                transfer.ExpireTime = transfer.CreateTime.AddMinutes(_Setting.TransferExpiration);

                                _TransferCollection.Create(transfer);

                                if (!transfer.Id.Equals(Guid.Empty))
                                {
                                    // Send mail
                                    // TODO
                                    res = transfer;
                                }
                                session.CommitTransactionAsync().Wait();
                            }
                            catch (Exception)
                            {
                                session.AbortTransactionAsync().Wait();
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    _Setting.Message.SetMessage("Không tìm thấy tài khoản người nhận!");
                }
            }
            else
            {
                _Setting.Message.SetMessage("Không tìm thấy tài khoản người gửi!");
            }

            return res;
        }

        public bool UpdateBankAccount(Guid userId, BankAccountType bankAccountType, BankAccount bankAccount)
        {
            var res = false;
            var filter = new UserFilter() { Id = userId };
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                if (bankAccountType == BankAccountType.CheckingAccount)
                {
                    var detail = details.FirstOrDefault().CheckingAccount;
                    if (detail.Id == bankAccount.Id)
                    {
                        detail.Description = bankAccount.Description;
                        detail.Name = bankAccount.Name;

                        res = _UserCollection.UpdateCheckingAccount(filter, detail) >= 0;
                    }
                }
                else
                {
                    var detail = details.FirstOrDefault().SavingsAccounts.FirstOrDefault(x => x.Id == bankAccount.Id);

                    if (detail != null)
                    {
                        detail.Description = bankAccount.Description;
                        detail.Name = bankAccount.Name;

                        res = _UserCollection.UpdateSavingsAccount(filter, detail) >= 0;
                    }
                }
            }
            return res;
        }

        public bool UpdatePayee(Guid userId, Payee payee)
        {
            var res = false;
            var filter = new UserFilter() { Id = userId };
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault().Payees.FirstOrDefault(x => x.Id == payee.Id);

                if (detail != null)
                {
                    detail.MnemonicName = payee.MnemonicName;

                    res = _UserCollection.UpdatePayee(userId, detail) >= 0;
                }
            }
            return res;
        }

        public bool UpdateUser(User user)
        {
            var res = false;
            var details = _UserCollection.Get(new UserFilter() { Id = user.Id });

            if (details.Any())
            {
                var detail = details.FirstOrDefault();

                detail.Name = user.Name;
                detail.Gender = user.Gender;
                detail.Address = user.Address;

                res = _UserCollection.Replace(detail) >= 0;
            }
            return res;
        }
    }
}
