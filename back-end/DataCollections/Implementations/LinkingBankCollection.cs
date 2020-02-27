using InternetBanking.Daos;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InternetBanking.DataCollections.Implementations
{
    public class MongoLinkingBankCollection : ILinkingBankCollection
    {
        private IMongoCollection<LinkingBank> _Collection;
        private MongoDBClient _MongoDBClient;
        public MongoLinkingBankCollection(MongoDBClient mongoDBClient)
        {
            _MongoDBClient = mongoDBClient;
            _Collection = mongoDBClient.GetCollection<LinkingBank>("LinkingBanks");
        }

        public void Create(LinkingBank employeeInfo)
        {

            _Collection.InsertOne(employeeInfo);
        }

        public async void CreateTransaction(LinkingBank employee)
        {
            using (var session = await _MongoDBClient.StartSessionAsync())
            {
                session.StartTransaction();
                try
                {
                    _Collection.InsertOne(employee);

                    await session.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    await session.AbortTransactionAsync();
                    throw;
                }
            }
        }

        public IEnumerable<LinkingBank> Get(LinkingBankFilter employeeFilter)
        {
            FilterDefinition<LinkingBank> filter = Builders<LinkingBank>.Filter.Empty;
            List<FilterDefinition<LinkingBank>> ops = new List<FilterDefinition<LinkingBank>>(10);
            if (!employeeFilter.Id.Equals(Guid.Empty))
                ops.Add(Builders<LinkingBank>.Filter.Eq(x => x.Id, employeeFilter.Id));

            if (!string.IsNullOrEmpty(employeeFilter.Name))
                ops.Add(Builders<LinkingBank>.Filter.Eq(x => x.Name, employeeFilter.Name));

            if (!string.IsNullOrEmpty(employeeFilter.Code))
                ops.Add(Builders<LinkingBank>.Filter.Eq(x => x.Code, employeeFilter.Code));

            if (ops.Count > 0)
                filter = Builders<LinkingBank>.Filter.And(ops);

            SortDefinition<LinkingBank> sort = null;
            FindOptions<LinkingBank, LinkingBank> options = null;

            options = new FindOptions<LinkingBank, LinkingBank>() { Sort = sort };

            var task = _Collection.Find(filter);
            return task.ToEnumerable();
        }

        public long Replace(LinkingBank employee)
        {
            FilterDefinition<LinkingBank> filter = Builders<LinkingBank>.Filter.Eq(x => x.Id, employee.Id);
            var res = _Collection.FindOneAndReplace(filter, employee);
            if (res != null)
                return 1;
            return 0;
        }

        public LinkingBank GetById(Guid id)
        {
            FilterDefinition<LinkingBank> filter = Builders<LinkingBank>.Filter.Eq(x => x.Id, id);

            SortDefinition<LinkingBank> sort = null;
            ProjectionDefinition<LinkingBank> projection = null;
            FindOptions<LinkingBank, LinkingBank> options = null;

            options = new FindOptions<LinkingBank, LinkingBank>() {  Sort = sort };

            var task = _Collection.Find(filter);
            return task.FirstOrDefault();
        }

        public long Update(LinkingBank employee)
        {
            var filter = Builders<LinkingBank>.Filter.Where(x => x.Id == employee.Id);
            Task<UpdateResult> res = null;


            var data = Builders<LinkingBank>.Update
                .Set(f => f.Name, employee.Name);

            res = _Collection.UpdateOneAsync(filter, data);

            return res != null ? res.Result.ModifiedCount : 0;
        }

        public long Delete(Guid id)
        {
            FilterDefinition<LinkingBank> filter = Builders<LinkingBank>.Filter.Eq(x => x.Id, id);
            var res = _Collection.DeleteOne(filter);
            return res != null ? res.DeletedCount : 0;
        }
    }
}
