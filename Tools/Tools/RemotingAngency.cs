using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    public class RemotingAngency
    {
        private StringBuilder Uri;
        private int Port = 11542;
        private static object _syncRoot = new object();
        private static RemotingAngency _angency = null;
        public static RemotingAngency GetRemoting()
        {
            if (_angency == null) {
                lock (_syncRoot) {
                    Interlocked.CompareExchange(ref _angency,new RemotingAngency(),null);
                }
            }
            return _angency;
        }
        private RemotingAngency()
        {
            string ip = System.Configuration.ConfigurationManager.AppSettings["XlettlerService"];
            if (string.IsNullOrEmpty(ip)) throw new Exception("IP is Empty!");
            Uri = new StringBuilder();
            Uri.Append("net.tcp://").Append(ip).Append(":").Append(Port);
        }
        public T GetProxy<T>()where T:class
        {
            string ipendport=GetRemotingUri.GetUri<T>(Uri.ToString());
            NetTcpBinding tcpBinding = new NetTcpBinding();
            tcpBinding.MaxReceivedMessageSize = 2147483647;
            tcpBinding.MaxBufferSize = 2147483647;
            NetTcpSecurity security = new NetTcpSecurity();
            security.Mode = SecurityMode.None;
            security.Message.ClientCredentialType = MessageCredentialType.None;
            tcpBinding.Security = security;
            var channelFactory = new ChannelFactory<T>(tcpBinding, ipendport);
            var proxy = channelFactory.CreateChannel();
            return proxy;
        }
    }
}
