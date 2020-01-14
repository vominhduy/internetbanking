using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Daos
{
    public class MongoDBClient
    {
        private MongoClient _client;
        private IMongoDatabase _db;
        private string _url;
        private string _dbName;

        public MongoDBClient(string url, string dbName)
        {
            _url = url;
            _dbName = dbName;
            Connect();
        }

        private void Connect()
        {
            Debug.Assert(_db == null);
            //Create a default mongo object. This handles our connections to the database.
            //By default, this will connect to localhost, 
            //port 27017 which we already have running from earlier.
            _client = new MongoClient(_url);

            //Get the blog database. If it doesn't exist, that's ok because MongoDB will create it 
            //for us when we first use it. Awesome!!!
            _db = _client.GetDatabase(_dbName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            Debug.Assert(_db != null);
            return _db.GetCollection<T>(name);
        }

        public async Task<IClientSessionHandle> StartSessionAsync()
        {
            return await _client.StartSessionAsync();
        }
    }
}
