using InternetBanking.Daos;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InternetBanking.DataCollections.Implementations
{
    public class MongoDeptReminderCollection : IDeptReminderCollection
    {
        private IMongoCollection<DeptReminder> _Collection;
        private MongoDBClient _MongoDBClient;
        public MongoDeptReminderCollection(MongoDBClient mongoDBClient)
        {
            _MongoDBClient = mongoDBClient;
            _Collection = mongoDBClient.GetCollection<DeptReminder>("DeptReminders");
        }

        public void Create(DeptReminder deptReminder)
        {
            _Collection.InsertOne(deptReminder);
        }

        public IEnumerable<DeptReminder> GetMany(DeptReminderFilter deptReminderFilter)
        {
            FilterDefinition<DeptReminder> filter = Builders<DeptReminder>.Filter.Empty;
            List<FilterDefinition<DeptReminder>> ops = new List<FilterDefinition<DeptReminder>>(10);
            if (!deptReminderFilter.Id.Equals(Guid.Empty))
                ops.Add(Builders<DeptReminder>.Filter.Eq(x => x.Id, deptReminderFilter.Id));

            if (!string.IsNullOrEmpty(deptReminderFilter.AccountNumberRecipient))
                ops.Add(Builders<DeptReminder>.Filter.Eq(x => x.Recipient.AccountNumber, deptReminderFilter.AccountNumberRecipient));

            if (!string.IsNullOrEmpty(deptReminderFilter.AccountNumberRequestor))
                ops.Add(Builders<DeptReminder>.Filter.Eq(x => x.Requestor.AccountNumber, deptReminderFilter.AccountNumberRequestor));

            if (ops.Count > 0)
                filter = Builders<DeptReminder>.Filter.And(ops);

            SortDefinition<DeptReminder> sort = null;
            FindOptions<DeptReminder, DeptReminder> options = null;

            options = new FindOptions<DeptReminder, DeptReminder>() { Projection = null, Sort = sort };

            return _Collection.FindSync(filter, options).ToEnumerable();
        }

        public long Replace(DeptReminder deptReminder)
        {
            FilterDefinition<DeptReminder> filter = Builders<DeptReminder>.Filter.Eq(x => x.Id, deptReminder.Id);
            var res = _Collection.FindOneAndReplace(filter, deptReminder);
            if (res != null)
                return 1;
            return 0;
        }

        public DeptReminder GetById(Guid id)
        {
            FilterDefinition<DeptReminder> filter = Builders<DeptReminder>.Filter.Eq(x => x.Id, id);

            SortDefinition<DeptReminder> sort = null;
            ProjectionDefinition<DeptReminder> projection = null;
            FindOptions<DeptReminder, DeptReminder> options = null;

            options = new FindOptions<DeptReminder, DeptReminder>() { Projection = projection, Sort = sort };

            return _Collection.FindAsync(filter, options).Result.FirstOrDefault();
        }


        public long Delete(Guid id)
        {
            FilterDefinition<DeptReminder> filter = Builders<DeptReminder>.Filter.Eq(x => x.Id, id);
            var res = _Collection.DeleteOne(filter);
            return res != null ? res.DeletedCount : 0;
        }
    }
}
