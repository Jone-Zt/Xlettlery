using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuleUtility
{
    public class RuleGenerateGame
    {
         public static object _lock = new object();
        public static int count = 1;
        public static long GetGameID()
        {
            lock (_lock)
            {
                Interlocked.CompareExchange(ref count, 1, 10000);
                long result=DateTime.Now.ToFileTimeUtc()+count;
                Interlocked.Increment(ref count);
                return result;
            }
        }
        public static object _sync = new object();
        public static string Update_CacheID(string linkId)
        {
            lock (_sync)
            {
                return linkId + DateTime.Now.ToString("yyyyMMdd");
            }
        }
    }
}
