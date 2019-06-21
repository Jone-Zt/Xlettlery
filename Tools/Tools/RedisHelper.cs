using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
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
                if (string.IsNullOrEmpty(_redisStr)) {
                    _redisStr = "127.0.0.1";
                }
                _redisClient = new RedisClient(_redisStr, 6379);
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
            if (!_redisClient.SetValueIfNotExists(key, value))
            {
                _redisClient.Del(key);
            }
            _redisClient.SetValue(key, value, timeOut);
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
    }
}
