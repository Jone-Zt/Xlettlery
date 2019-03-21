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
            foreach(PropertyInfo p in pi){  
                MethodInfo mi = p.GetGetMethod();  
                if (mi!=null && mi.IsPublic)  
                {  
                   map.Add(p.Name, mi.Invoke(o, new Object[] { }));  
                }  
            }  
            return map;
        }  

    }
}
