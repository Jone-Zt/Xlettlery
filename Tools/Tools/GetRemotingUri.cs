using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class GetRemotingUri
    {
        public static string GetUri<T>(string Url)
        {
            var Name = typeof(T).Name;
            StringBuilder builder = new StringBuilder();
            builder.Append(Url).Append("/").Append(Name);
            return builder.ToString();
        }
    }
}
