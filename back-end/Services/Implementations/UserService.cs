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
        private ISetting _Setting;
        public UserService(ISetting setting, IUserCollection userCollection)
        {
            _UserCollection = userCollection;
            _Setting = setting;
        }

        public bool AddSavingsAccount(UserFilter userFilter, BankAccount bankAccount)
        {
            var res = false;
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
                    userFilter.Name = null;
                    userFilter.Id = detail.Id;
                    res = _UserCollection.AddSavingsAccount(userFilter, bankAccount) >= 0;
                }
            }

            return res;
        }

        public User AddUser(User user)
        {
            _UserCollection.Create(user);
            return user;
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

                    detail.Description = bankAccount.Description;
                    detail.Name = bankAccount.Name;

                    res = _UserCollection.UpdateCheckingAccount(filter, detail) >= 0;
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
