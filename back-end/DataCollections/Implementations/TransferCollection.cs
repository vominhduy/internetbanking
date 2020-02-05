using InternetBanking.Daos;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InternetBanking.DataCollections.Implementations
{
    public class MongoTransferCollection : ITransferCollection
    {
        private IMongoCollection<Transfer> _Collection;
        private MongoDBClient _MongoDBClient;
        public MongoTransferCollection(MongoDBClient mongoDBClient)
        {
            _MongoDBClient = mongoDBClient;
            _Collection = mongoDBClient.GetCollection<Transfer>("Transfers");
        }

        public void Create(Transfer Transfer)
        {
            _Collection.InsertOne(Transfer);
        }

        public IEnumerable<Transfer> GetMany(TransferFilter transferFilter)
        {
            FilterDefinition<Transfer> filter = Builders<Transfer>.Filter.Empty;
            List<FilterDefinition<Transfer>> ops = new List<FilterDefinition<Transfer>>(10);
            if (!transferFilter.Id.Equals(Guid.Empty))
                ops.Add(Builders<Transfer>.Filter.Eq(x => x.Id, transferFilter.Id));

            if (!string.IsNullOrEmpty(transferFilter.DestinationAccountNumber))
                ops.Add(Builders<Transfer>.Filter.Eq(x => x.DestinationAccountNumber, transferFilter.DestinationAccountNumber));

            if (transferFilter.DestinationLinkingBankId != Guid.Empty)
                ops.Add(Builders<Transfer>.Filter.Eq(x => x.DestinationLinkingBankId, transferFilter.DestinationLinkingBankId));

            if (!string.IsNullOrEmpty(transferFilter.SourceAccountNumber))
                ops.Add(Builders<Transfer>.Filter.Eq(x => x.SourceAccountNumber, transferFilter.SourceAccountNumber));

            if (transferFilter.SourceLinkingBankId != Guid.Empty)
                ops.Add(Builders<Transfer>.Filter.Eq(x => x.SourceLinkingBankId, transferFilter.SourceLinkingBankId));


            if (ops.Count > 0)
                filter = Builders<Transfer>.Filter.And(ops);

            SortDefinition<Transfer> sort = null;
            FindOptions<Transfer, Transfer> options = null;

            options = new FindOptions<Transfer, Transfer>() { Projection = null, Sort = sort };

            return _Collection.FindSync(filter, options).ToEnumerable();
        }

        public long Replace(Transfer Transfer)
        {
            FilterDefinition<Transfer> filter = Builders<Transfer>.Filter.Eq(x => x.Id, Transfer.Id);
            var res = _Collection.FindOneAndReplace(filter, Transfer);
            if (res != null)
                return 1;
            return 0;
        }

        public Transfer GetById(Guid id)
        {
            FilterDefinition<Transfer> filter = Builders<Transfer>.Filter.Eq(x => x.Id, id);

            SortDefinition<Transfer> sort = null;
            ProjectionDefinition<Transfer> projection = null;
            FindOptions<Transfer, Transfer> options = null;

            options = new FindOptions<Transfer, Transfer>() { Projection = projection, Sort = sort };

            return _Collection.FindAsync(filter, options).Result.FirstOrDefault();
        }


        public long Delete(Guid id)
        {
            FilterDefinition<Transfer> filter = Builders<Transfer>.Filter.Eq(x => x.Id, id);
            var res = _Collection.DeleteOne(filter);
            return res != null ? res.DeletedCount : 0;
        }
    }
}
