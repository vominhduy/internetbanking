using InternetBanking.Daos;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InternetBanking.DataCollections.Implementations
{
    public class MongoEmployeeCollection : IEmployeeCollection
    {
        private IMongoCollection<Employee> _Collection;
        private MongoDBClient _MongoDBClient;
        public MongoEmployeeCollection(MongoDBClient mongoDBClient)
        {
            _MongoDBClient = mongoDBClient;
            _Collection = mongoDBClient.GetCollection<Employee>("Emmployees");
        }

        public void Create(Employee employeeInfo)
        {

            _Collection.InsertOne(employeeInfo);
        }

        public async void CreateTransaction(Employee employee)
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

        public IEnumerable<Employee> Get(EmployeeFilter employeeFilter)
        {
            FilterDefinition<Employee> filter = Builders<Employee>.Filter.Empty;
            List<FilterDefinition<Employee>> ops = new List<FilterDefinition<Employee>>(10);
            if (!employeeFilter.Id.Equals(Guid.Empty))
                ops.Add(Builders<Employee>.Filter.Eq(x => x.Id, employeeFilter.Id));

            if (!string.IsNullOrEmpty(employeeFilter.Name))
                ops.Add(Builders<Employee>.Filter.Eq(x => x.Name, employeeFilter.Name));

            if (ops.Count > 0)
                filter = Builders<Employee>.Filter.And(ops);

            SortDefinition<Employee> sort = null;
            FindOptions<Employee, Employee> options = null;

            options = new FindOptions<Employee, Employee>() { Projection = null, Sort = sort };

            return _Collection.FindSync(filter, options).ToEnumerable();
        }

        public long Replace(Employee employee)
        {
            FilterDefinition<Employee> filter = Builders<Employee>.Filter.Eq(x => x.Id, employee.Id);
            var res = _Collection.FindOneAndReplace(filter, employee);
            if (res != null)
                return 1;
            return 0;
        }

        public long Update(Employee employee)
        {
            var filter = Builders<Employee>.Filter.Where(x => x.Id == employee.Id);
            Task<UpdateResult> res = null;


            var data = Builders<Employee>.Update
                .Push(f => f.GroupIds, employee.GroupIds[0])
                .Set(f => f.Name, employee.Name);

            res = _Collection.UpdateOneAsync(filter, data);

            return res != null ? res.Result.ModifiedCount : 0;
        }

        public long Delete(Guid id)
        {
            FilterDefinition<Employee> filter = Builders<Employee>.Filter.Eq(x => x.Id, id);
            var res = _Collection.DeleteOne(filter);
            return res != null ? res.DeletedCount : 0;
        }
    }
}
