using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuleUtility
{
   public class RuleGenerateOrder
    {
        public static object _lock = new object();
        public static int count = 1;
        public static string GetOrderID()
        {
            lock (_lock)
            {
                Interlocked.CompareExchange(ref count, 1, 10000);
                StringBuilder builder = new StringBuilder();
                builder.Append(DateTime.Now.ToString("yyMMddHHmmss")).Append(count.ToString("0000"));
                Interlocked.Increment(ref count);
                return builder.ToString();
            }
        }
}
}
