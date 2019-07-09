using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class UntilsObjToDic
    {
        public static Dictionary<String, Object> ToMap(Object o)
        {
            Dictionary<String, Object> map = new Dictionary<string, object>();
            Type t = o.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();
                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, mi.Invoke(o, new Object[] { }));
                }
            }
            return map;
        }
        public static Dictionary<string, string> ProductDetailList(string ProductDetails)
        {
            if (string.IsNullOrWhiteSpace(ProductDetails))
            {
                return new Dictionary<string, string>();
            }
            try
            {
                var obj = JToken.Parse(ProductDetails);
            }
            catch (Exception)
            {
                throw new FormatException("ProductDetails不符合json格式.");
            }
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(ProductDetails);
        }
    }
}
