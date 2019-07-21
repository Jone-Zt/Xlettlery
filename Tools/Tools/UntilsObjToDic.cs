using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
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

        public static DataTable ListToDataTable<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
                return null;
            DataTable dt = new DataTable("tableName");
            //创建传入对象名称的列
            foreach (var item in list.FirstOrDefault().GetType().GetProperties())
            {
                dt.Columns.Add(item.Name);
            }
            //循环存储
            foreach (var item in list)
            {
                //新加行
                DataRow value = dt.NewRow();
                //根据DataTable中的值，进行对应的赋值
                foreach (DataColumn dtColumn in dt.Columns)
                {
                    int i = dt.Columns.IndexOf(dtColumn);
                    //基元元素，直接复制，对象类型等，进行序列化
                    //if (value.GetType().IsPrimitive)
                    //{
                        value[i] = item.GetType().GetProperty(dtColumn.ColumnName).GetValue(item);
                    //}
                    //else
                    //{
                    //    value[i] = JsonConvert.SerializeObject(item.GetType().GetProperty(dtColumn.ColumnName).GetValue(item));
                    //}
                }
                dt.Rows.Add(value);
            }
            return dt;
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
