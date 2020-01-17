using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Constants;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
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
        public UserService(ISetting setting, IUserCollection userCollection, ILinkingBankCollection linkingBankCollection)
        {
            _UserCollection = userCollection;
            _Setting = setting;
            _LinkingBankCollection = linkingBankCollection;
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
