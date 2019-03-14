using CallBackManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XlettlerCallBackService
{
    public class XletterCallBackManagment
    {
        private static XletterCallBackManagment _Xletter;
        private readonly static object _obj = new object();
        public static XletterCallBackManagment GetManagment()
        {
            if (_Xletter == null)
            {
                lock (_obj)
                {
                    Interlocked.CompareExchange(ref _Xletter, new XletterCallBackManagment(), null);
                }
            }
            return _Xletter;
        }
        private CallBackProcessServer pServer;
        public void Init()
        {
            try
            {
                LogTool.LogWriter.InitConfigFile(AppDomain.CurrentDomain.BaseDirectory + "\\Log4netConfigFile.xml");
                int rPort = 4902;
                string cPort = System.Configuration.ConfigurationManager.AppSettings["SERVICE_PORT"];
                if (!string.IsNullOrEmpty(cPort))
                    int.TryParse(cPort, out rPort);
                pServer = new CallBackProcessServer(rPort);
                pServer.InitHandles();
                pServer.Start();
                LogTool.LogWriter.WriteInfo("自定义回调服务已启动 监听端口:" + rPort);
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError(err.Message);
            }
        }
        public void Stop()
        {
           
        }
    }
}
