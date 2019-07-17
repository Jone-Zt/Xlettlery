using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public enum MethordType
    {
        POST,
        GET,
        POSTWITHJSON
    }
    public class HttpHelper
    {
        public static string SendRequest(string Url, SortedDictionary<string,object> data, MethordType methordType, Encoding encoding, string ContentType = "application/x-www-form-urlencoded")
        {
            string result = string.Empty;
            HttpClient client = null;
            try
            {
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase)) {
                    HttpClientHandler handler = new HttpClientHandler(); handler.AllowAutoRedirect = true; handler.UseCookies = true; 
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client = new HttpClient(handler); }
                else
                    client = new HttpClient();
                HttpResponseMessage message = null;
                string Data = GenerateStrWithByRef(data);
                if (methordType == MethordType.POST)
                {
                    byte[] bts = encoding.GetBytes(Data);
                    using (Stream dataStream = new MemoryStream(bts ?? new byte[0]))
                    {
                        using (HttpContent content = new StreamContent(dataStream))
                        {
                            content.Headers.Add("Content-Type", ContentType);
                            var task = client.PostAsync(Url, content);
                            message = task.Result;
                        }
                    }
                }
                else if (methordType == MethordType.GET)
                {
                    StringBuilder getData = new StringBuilder();
                    getData.Append(Url).Append("?").Append(Data ?? "");
                    var task = client.GetAsync(getData.ToString());
                    message = task.Result;
                }
                else if (methordType == MethordType.POSTWITHJSON)
                {
                   string Jsondata=JsonConvert.SerializeObject(data);
                    byte[] bts = encoding.GetBytes(Jsondata);
                    using (Stream dataStream = new MemoryStream(bts ?? new byte[0]))
                    {
                        using (HttpContent content = new StreamContent(dataStream))
                        {
                            ContentType = "application/json";
                            content.Headers.Add("Content-Type", ContentType);
                            var task = client.PostAsync(Url, content);
                            message = task.Result;
                        }
                    }
                }
                if (message != null && message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (message)
                    {
                        result = message.Content.ReadAsStringAsync().Result;
                    }
                }
                return result;
            }
            catch (Exception x)
            {
                return "";
            }
            finally
            {
                client.Dispose();
            }
        }
        public static SortedDictionary<string, object> GenerateData<R>(R t) where R : class
        {
            SortedDictionary<string, object> data = new SortedDictionary<string, object>();
            if (t == null) { return null; }
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (properties.Length <= 0) { return null; }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);
                if ((item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String")) && value != null) {
                    data.Add(name, value);
                }
                else { GenerateData(value); }
            }
            return data;
        }
        private static string GenerateStrWithByRef(SortedDictionary<string, object> keyValues)
        {
            if (keyValues == null || keyValues.Count == 0)
                return null;
            StringBuilder builder = new StringBuilder();
            var itemtor = keyValues.GetEnumerator();
            while (itemtor.MoveNext())
            {
                builder.Append(itemtor.Current.Key);
                builder.Append("=");
                builder.Append(itemtor.Current.Value??"");
                builder.Append("&");
            }
            if (builder.Length > 0) builder.Remove(builder.Length-1,1);
            return builder.ToString();
        }
    }
}
