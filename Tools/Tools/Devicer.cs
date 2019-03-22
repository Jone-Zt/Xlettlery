using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    [Serializable]
    public class DervierData
    {
        public string userid { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public string mobile { get; set; }
        public string content { get; set; }
        public string sendTime { get; set; }
        public string action { get; set; }
        public string extno { get; set; }
    }

    public class Devicer
    {
        public static void SendMsg(string Phone, string Content)
        {
            DervierData dervier = new DervierData()
            {
                userid = "19011",
                account = "K90306",
                password= "K90306",
                mobile=Phone,
                content=Content,
                action="send",
            };
           string postData=GetPostData(dervier);
            LogTool.LogWriter.WriteDebug($"短信通道请求:{postData}");
            try
            {
                string result = DoPost("http://sms.any163.cn:8888/sms.aspx", postData, 60000);
                LogTool.LogWriter.WriteDebug($"短信通道响应:{result}");
            }
            catch (Exception err) {
                LogTool.LogWriter.WriteError("短信通道响应错误",err);
                throw err;
            }
        }
        public static string GetPostData<K>(K k)
        {
            return GetData(ReflectObject(k, false, string.Empty));
        }
        private static string GetData(SortedDictionary<string, string> Data)
        {
            var items = Data.GetEnumerator();
            StringBuilder _sb = new StringBuilder();
            while (items.MoveNext())
            {
                _sb.Append(items.Current.Key);
                _sb.Append("=");
                _sb.Append(items.Current.Value);
                _sb.Append("&");
            }
            _sb.Remove(_sb.Length - 1, 1);
            return _sb.ToString();
        }
        private static SortedDictionary<string, string> ReflectObject<R>(R r, bool isSign, string signName)
        {
            SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
            Type type = r.GetType();
            if (type != null)
            {
                PropertyInfo[] props = type.GetProperties(System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.CreateInstance);
                foreach (PropertyInfo item in props)
                {
                    var obj = item.GetValue(r, null);
                    if (isSign)
                    {
                        if (obj != null && item.Name != signName)
                        {
                            dic.Add(item.Name, obj.ToString());
                        }
                    }
                    else
                    {
                        if (obj != null)
                        {
                            dic.Add(item.Name, obj.ToString());
                        }
                    }
                }
            }
            return dic;
        }
        public static string DoPost(string url, string postData, int timeOut)
        {
            StringBuilder sb = new StringBuilder();
            byte[] pars = Encoding.UTF8.GetBytes(postData);
            if (url.ToLower().StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => {
                            return true;
                        });
            }
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            req.ContentLength = pars.Length;
            req.Timeout = timeOut;
            using (Stream reqStm = req.GetRequestStream())
            {
                reqStm.Write(pars, 0, pars.Length);
                var rep = req.GetResponse();
                long readerCount = rep.ContentLength;
                using (Stream repStm = rep.GetResponseStream())
                {
                    int num = -1;
                    do
                    {
                        byte[] bts = new byte[1024];
                        num = repStm.Read(bts, 0, bts.Length);
                        byte[] result = byteCut(bts, (byte)0x00);
                        sb.Append(Encoding.UTF8.GetString(result));

                    } while (num != 0);
                }
            }
            return sb.ToString();
        }
        private static byte[] byteCut(byte[] b, byte cut)
        {
            List<byte> list = new List<byte>();
            list.AddRange(b);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == cut)
                    list.RemoveAt(i);
            }
            return list.ToArray();
        }
    }
}
