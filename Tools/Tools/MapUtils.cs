using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Tools
{
    public class MapUtils
    {
        public static Dictionary<string, object> ObjectToMap(object obj, bool isIgnoreNull = false)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();

            Type t = obj.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    if (m.Invoke(obj, new object[] { }) != null || !isIgnoreNull)
                    {
                        map.Add(p.Name, m.Invoke(obj, new object[] { }));
                    }
                }
            }
            return map;
        }
    }
}
