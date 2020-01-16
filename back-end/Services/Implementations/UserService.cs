using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
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

        public IEnumerable<User> GetUsers(UserFilter employeeFilter)
        {
            return _UserCollection.Get(employeeFilter);
        }

        public bool UpdateUser(User user)
        {
            return _UserCollection.Replace(user) >= 0;
        }
    }
}
