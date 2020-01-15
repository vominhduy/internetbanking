using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using System.Collections.Generic;

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
        public IEnumerable<User> GetUsers(UserFilter employeeFilter)
        {
            return _UserCollection.Get(employeeFilter);
        }
    }
}
