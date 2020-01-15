﻿using InternetBanking.Daos;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InternetBanking.DataCollections.Implementations
{
    public class MongoUserCollection : IUserCollection
    {
        private IMongoCollection<User> _Collection;
        private MongoDBClient _MongoDBClient;
        public MongoUserCollection(MongoDBClient mongoDBClient)
        {
            _MongoDBClient = mongoDBClient;
            _Collection = mongoDBClient.GetCollection<User>("Users");
        }

        public void Create(User userInfo)
        {

            _Collection.InsertOne(userInfo);
        }

        public async void CreateTransaction(User user)
        {
            using (var session = await _MongoDBClient.StartSessionAsync())
            {
                session.StartTransaction();
                try
                {
                    _Collection.InsertOne(user);

                    await session.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    await session.AbortTransactionAsync();
                    throw;
                }
            }
        }

        public IEnumerable<User> Get(UserFilter userFilter)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Empty;
            List<FilterDefinition<User>> ops = new List<FilterDefinition<User>>(10);
            if (!userFilter.Id.Equals(Guid.Empty))
                ops.Add(Builders<User>.Filter.Eq(x => x.Id, userFilter.Id));

            if (!string.IsNullOrEmpty(userFilter.Name))
                ops.Add(Builders<User>.Filter.Eq(x => x.Name, userFilter.Name));

            if (ops.Count > 0)
                filter = Builders<User>.Filter.And(ops);

            SortDefinition<User> sort = null;
            FindOptions<User, User> options = null;

            options = new FindOptions<User, User>() { Projection = null, Sort = sort };

            return _Collection.FindSync(filter, options).ToEnumerable();
        }

        public long Replace(User user)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
            var res = _Collection.FindOneAndReplace(filter, user);
            if (res != null)
                return 1;
            return 0;
        }

        public long Update(User user)
        {
            var filter = Builders<User>.Filter.Where(x => x.Id == user.Id);
            Task<UpdateResult> res = null;


            var data = Builders<User>.Update
                .Set(f => f.Name, user.Name);

            res = _Collection.UpdateOneAsync(filter, data);

            return res != null ? res.Result.ModifiedCount : 0;
        }

        public long Delete(Guid id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var res = _Collection.DeleteOne(filter);
            return res != null ? res.DeletedCount : 0;
        }
    }
}