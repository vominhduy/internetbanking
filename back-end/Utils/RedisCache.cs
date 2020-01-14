using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace InternetBanking.Utils
{
    public class RedisCacheTransaction
    {
        private ITransaction _transaction;
        public RedisCacheTransaction(ITransaction transaction)
        {
            _transaction = transaction;
        }

        public bool Execute()
        {
            return _transaction.Execute();
        }

        public Task<double> IncrementAsync(string key, double value)
        {
            return _transaction.StringIncrementAsync(key, value);
        }

        public Task StringSetAsync(string key, string value)
        {
            return _transaction.StringSetAsync(key, value);
        }

        public Task StringSetAsync(string key, double value)
        {
            return _transaction.StringSetAsync(key, value);
        }
    }

    public class RedisCache
    {
        ConnectionMultiplexer _redis;
        string _name;

        public RedisCache(string servers, string name)
        {
            _name = string.IsNullOrEmpty(name) ? string.Empty : $"{name}.";
            _redis = ConnectionMultiplexer.Connect(servers);
        }

        private string GetKey(string key)
        {
            return $"{_name}{key}";
        }

        public bool Exists(string key)
        {
            IDatabase db = _redis.GetDatabase();
            return db.KeyExists(GetKey(key));
        }

        public void Delete(string key)
        {
            IDatabase db = _redis.GetDatabase();
            db.KeyDelete(GetKey(key));
        }

        public bool StringTryGet(string key, out string value)
        {
            IDatabase db = _redis.GetDatabase();

            RedisValue v = db.StringGet(GetKey(key));

            if (v.HasValue)
            {
                value = v;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public string StringGet(string key)
        {
            IDatabase db = _redis.GetDatabase();

            return db.StringGet(GetKey(key));
        }

        public string[] StringGetStrings(string[] keys)
        {
            IDatabase db = _redis.GetDatabase();

            RedisKey[] redisKeys = GetRedisKeys(keys);
            RedisValue[] redisValues = db.StringGet(redisKeys);

            string[] values = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                values[i] = redisValues[i];
            }

            return values;
        }

        public double[] StringGetDoubles(string[] keys)
        {
            IDatabase db = _redis.GetDatabase();

            RedisKey[] redisKeys = GetRedisKeys(keys);
            RedisValue[] redisValues = db.StringGet(redisKeys);

            double[] values = new double[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                values[i] = (double)redisValues[i];
            }

            //var tran = db.CreateTransaction();

            return values;
        }

        public void StringSet(string key, string value)
        {
            StringSetIn(key, value, null);
        }

        public void StringSet(string key, string value, TimeSpan tsExpire)
        {
            StringSetIn(key, value, tsExpire);
        }

        public void StringSetIn(string key, string value, TimeSpan? tsExpire)
        {
            IDatabase db = _redis.GetDatabase();

            db.StringSet(GetKey(key), value, tsExpire);
        }

        public void StringSet(IDictionary<string, string> keyValues)
        {
            IDatabase db = _redis.GetDatabase();
            KeyValuePair<RedisKey, RedisValue>[] kvs = new KeyValuePair<RedisKey, RedisValue>[keyValues.Count];
            int i = 0;
            foreach (var kv in keyValues)
            {
                kvs[i] = new KeyValuePair<RedisKey, RedisValue>(GetKey(kv.Key), kv.Value);
                i++;
            }
            db.StringSet(kvs);
        }

        public T Get<T>(string key)
        {
            return ConvertValue<T>(StringGet(key));
        }

        public bool TryGet<T>(string key, out T value)
        {
            string svalue;

            if (StringTryGet(key, out svalue))
            {
                value = ConvertValue<T>(svalue);
                return true;
            }

            value = default(T);
            return false;
        }

        public void Set<T>(string key, T value)
        {
            SetIn(key, value, null);
        }

        public void Set<T>(string key, T value, TimeSpan tsExpire)
        {
            this.SetIn(key, value, tsExpire);
        }

        public void SetIn<T>(string key, T value, TimeSpan? tsExpire)
        {
            string svalue;

            Type valueType = typeof(T);

            if (valueType.GetTypeInfo().IsValueType || (valueType == typeof(string)))
            {
                svalue = value.ToString();
            }
            else
            {
                svalue = JsonSerializer.Serialize(value);
            }

            StringSetIn(key, svalue, tsExpire);
        }

        private T ConvertValue<T>(string value)
        {
            if (value == null)
                return default;

            Type valueType = typeof(T);

            if (valueType == typeof(string))
                return (T)(object)value;

            if (valueType.GetTypeInfo().IsValueType)
            {
                return (T)Convert.ChangeType(value, valueType);
            }
            if (value == null)
                return default(T);
            return JsonSerializer.Deserialize<T>(value);
        }

        private RedisKey[] GetRedisKeys(string[] keys)
        {
            RedisKey[] redisKeys = new RedisKey[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                redisKeys[i] = GetKey(keys[i]);
            }

            return redisKeys;
        }

        public RedisCacheTransaction CreateTransaction()
        {
            IDatabase db = _redis.GetDatabase();

            return new RedisCacheTransaction(db.CreateTransaction());
        }
    }
}
