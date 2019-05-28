using ChannelManagement;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using Tools;
using XlettlerRealization;

namespace XlettlerService
{
    public class XletterManagment
    {
        private StringBuilder Uri;
        private int Port = 11542;
        private IList<ServiceHost> ServiceHosts;
        private XletterManagment()
        {
            ServiceHosts = new List<ServiceHost>();
            string ip = System.Configuration.ConfigurationManager.AppSettings["XlettlerService"];
            if (string.IsNullOrEmpty(ip)) throw new Exception("IP is Empty!");
            Uri = new StringBuilder();
            Uri.Append("net.tcp://").Append(ip).Append(":").Append(Port);
        }
        private static XletterManagment _Xletter;
        private readonly static object _obj = new object();
        public static XletterManagment GetManagment()
        {
            if (_Xletter == null)
            {
                lock (_obj)
                {
                    Interlocked.CompareExchange(ref _Xletter, new XletterManagment(), null);
                }
            }
            return _Xletter;
        }

        public void Init()
        {
            try
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                ServiceHost host = new ServiceHost(typeof(UserService));
                ServiceHost pay = new ServiceHost(typeof(PayService));
                ServiceHost setting = new ServiceHost(typeof(SettingService));
                ServiceHost shortMessage = new ServiceHost(typeof(ShortMessageService));
                ServiceHost Notify = new ServiceHost(typeof(ChannelNotifyService));
                ServiceHost Auth = new ServiceHost(typeof(AuthorizeServices));
                CreateRemoting<IAuthorizeInterface>(Auth);
                CreateRemoting<IUserInterface>(host);
                CreateRemoting<IPayInterface>(pay);
                CreateRemoting<IShortMessageInterface>(shortMessage);
                CreateRemoting<ISettingInterface>(setting);
                CreateRemoting<IChannelNotify>(Notify);
                StartServer();
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError("初始化WCF服务失败",err);
            }
            try
            {
                ChannelProtocolManage.GetManagment().Init();
            }
            catch(Exception err)
            {
                LogTool.LogWriter.WriteError("初始化第三方支付协议失败",err);
            }
        }
        private void CreateRemoting<T>(ServiceHost host)
        {
            NetTcpBinding binding = new NetTcpBinding();
            ServiceEndpoint userEndpoint = host.AddServiceEndpoint(typeof(T), binding, GetRemotingUri.GetUri<T>(Uri.ToString()));
            ServiceHosts.Add(host);
        }

        public void Stop()
        {
            StopServer();
        }
        private void StopServer()
        {
            try
            {
                foreach (ServiceHost item in ServiceHosts)
                {
                    item.Close();
                }
                ServiceHosts = null;
            }
            catch (Exception err) {
                LogTool.LogWriter.WriteError("服务停止失败:"+err.Message);
            }
        }
        private void StartServer() {
            try
            {
                foreach (ServiceHost item in ServiceHosts)
                {
                    item.Open();
                }
            }
            catch (Exception err) {
                LogTool.LogWriter.WriteError("服务开启失败:"+err.Message);
            }
        }
    }
}
