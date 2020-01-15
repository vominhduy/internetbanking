using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;

namespace InternetBanking.DataCollections
{
    public interface IUserCollection
    {
        void Create(User userInfo);
        IEnumerable<User> Get(UserFilter userFilter);
        void CreateTransaction(User userInfo);
        long Replace(User userInfo);
        long Update(User userInfo);
        long Delete(Guid id);
    }
}
