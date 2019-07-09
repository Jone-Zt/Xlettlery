using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuleUtility
{
    public class RuleGenerateGameWithGameID
    {
        public static object _lock = new object();
        public static int count = 1;
        public static long GetGameID()
        {
            lock (_lock)
            {
                Interlocked.CompareExchange(ref count, 1, 10000);
                long result =long.Parse(DateTime.Now.ToString("mmss")) + count;
                Interlocked.Increment(ref count);
                return result;
            }
        }
        private static object _sync = new object();
        public static string GetGameCode(string No, string date,string type,int j)
        {
            lock (_sync)
            {
                DateTime time=DateTime.Parse(date);
                return Tools.NPinYinHelper.TraformPinYin(No) + time.ToString("yyyyMMdd") + type+j;
            }
        }
    }
}
