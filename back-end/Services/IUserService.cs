using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System.Collections.Generic;

namespace InternetBanking.Services
{
    public interface IUserService
    {
        public IEnumerable<User> GetUsers(UserFilter userFilter);
    }
}
