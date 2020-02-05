using InternetBanking.Daos;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InternetBanking.DataCollections.Implementations
{
    public class MongoTransactionCollection : ITransactionCollection
    {
        private IMongoCollection<Transaction> _Collection;
        private MongoDBClient _MongoDBClient;
        public MongoTransactionCollection(MongoDBClient mongoDBClient)
        {
            _MongoDBClient = mongoDBClient;
            _Collection = mongoDBClient.GetCollection<Transaction>("Transactions");
        }

        public void Create(Transaction transaction)
        {
            _Collection.InsertOne(transaction);
        }

        public IEnumerable<Transaction> GetMany(TransactionFilter transactionFilter)
        {
            FilterDefinition<Transaction> filter = Builders<Transaction>.Filter.Empty;
            List<FilterDefinition<Transaction>> ops = new List<FilterDefinition<Transaction>>(10);
            if (transactionFilter.Id != Guid.Empty)
                ops.Add(Builders<Transaction>.Filter.Eq(x => x.Id, transactionFilter.Id));

            if (transactionFilter.ReferenceId != Guid.Empty)
                ops.Add(Builders<Transaction>.Filter.Eq(x => x.ReferenceId, transactionFilter.ReferenceId));

            if (!string.IsNullOrEmpty(transactionFilter.Otp))
                ops.Add(Builders<Transaction>.Filter.Eq(x => x.Otp, transactionFilter.Otp));

            if (ops.Count > 0)
                filter = Builders<Transaction>.Filter.And(ops);

            SortDefinition<Transaction> sort = null;
            FindOptions<Transaction, Transaction> options = null;

            options = new FindOptions<Transaction, Transaction>() { Projection = null, Sort = sort };

            return _Collection.FindSync(filter, options).ToEnumerable();
        }

        public long Replace(Transaction transaction)
        {
            FilterDefinition<Transaction> filter = Builders<Transaction>.Filter.Eq(x => x.Id, transaction.Id);
            var res = _Collection.FindOneAndReplace(filter, transaction);
            if (res != null)
                return 1;
            return 0;
        }

        public Transaction GetById(Guid id)
        {
            FilterDefinition<Transaction> filter = Builders<Transaction>.Filter.Eq(x => x.Id, id);

            SortDefinition<Transaction> sort = null;
            ProjectionDefinition<Transaction> projection = null;
            FindOptions<Transaction, Transaction> options = null;

            options = new FindOptions<Transaction, Transaction>() { Projection = projection, Sort = sort };

            return _Collection.FindAsync(filter, options).Result.FirstOrDefault();
        }


        public long Delete(Guid id)
        {
            FilterDefinition<Transaction> filter = Builders<Transaction>.Filter.Eq(x => x.Id, id);
            var res = _Collection.DeleteOne(filter);
            return res != null ? res.DeletedCount : 0;
        }
    }
}
