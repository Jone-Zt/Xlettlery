using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Tools
{
    public class RedisHelper
    {
        private RedisClient _redisClient;
        private string _redisStr;
        private RedisHelper()
        {
            try
            {
                _redisStr = System.Configuration.ConfigurationManager.AppSettings["RedisService"];
                if (string.IsNullOrEmpty(_redisStr)) {
                    _redisStr = "127.0.0.1";
                }
                _redisClient = new RedisClient(_redisStr, 6379,"micaikeji",0);
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError("Redis初始化失败!" + err.Message);
            }
        }
        private static readonly object _sync = new object();
        private static RedisHelper _redis;
        public static RedisHelper GetManger()
        {
            if (_redis == null)
            {
                lock (_sync)
                {
                    Interlocked.CompareExchange(ref _redis, new RedisHelper(), null);
                }
            }
            return _redis;
        }
        public bool Set(string key,string value,TimeSpan timeOut)
        {
            long count= _redisClient.Exists(key);
            if (count > 0)
            {
                _redisClient.Del(key);
            }
            _redisClient.Set(key, value, timeOut);
            return true;
        }
        public TimeSpan? GetTimeOut(string key)
        {
           return _redisClient.GetTimeToLive(key);
        }
        public string Get(string key)
        {
            byte[] bts=_redisClient.Get(key);
            if (bts != null && bts.Length > 0)
            {
              return Encoding.UTF8.GetString(bts);
            }
            return null;
        }
        public bool SetWithList<T>(string key,T t,TimeSpan timeOut) where T:class
        {
            return _redisClient.Set<T>(key, t, timeOut);
        }
        public T GetWithObject<T>(string key) where T : class
        {
           return _redisClient.Get<T>(key);
        }
        private static object _obj = new object();
        public List<T> GetWithList<T>()where T:class,new()
        {
            lock (_obj)
            {
                List<T> list = new List<T>();
                List<string> keys = _redisClient.GetAllKeys();
                var item = keys.GetEnumerator();
                while (item.MoveNext())
                {
                    if (item.Current.Contains(typeof(T).Name))
                    {
                        list.Add(_redisClient.Get<T>(item.Current));
                    }
                }
                return list;
            }
        }
    }
}
