using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChannelInterFace
{
    public abstract class TPayRechargeBase
    {
        public bool IsDebug { get; set; }
        public string AssemblyVersion { get; set; }
        public string ChannelTag { get; set; }
        public SESENT_ChannelProtocol MyProtocol { get; set; }
        public abstract void Stop();
        public abstract void InitData(XmlNode paramets);
        public string GetNotifyUrl(string channelID, string OrderID, string[] customerArges = null)
        {
            string notifyUrl = System.Configuration.ConfigurationManager.AppSettings["CallBackServer"];
            if (string.IsNullOrEmpty(notifyUrl))
                throw new Exception("多渠道服务未配置回调通知地址[CallBackServer]");
            if (!notifyUrl.EndsWith("/"))
                notifyUrl = notifyUrl + "/";
            else
                notifyUrl = notifyUrl + "Order/" + channelID + "/" + OrderID;
            if (customerArges != null && customerArges.Length > 0)
                notifyUrl = notifyUrl + "/" + string.Join("/", customerArges);
            return notifyUrl;
        }
        public virtual bool Notify(string OrderID,string[] pathArges, Dictionary<string, string> UrlArges, byte[] postBuffer,out string BackStr)
        {
            BackStr = "";
            return true;
        }
    }
}
