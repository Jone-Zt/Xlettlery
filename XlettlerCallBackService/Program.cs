using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace XlettlerCallBackService
{
    class Program
    {
        static void Main(string[] args)
        {
            LogTool.LogWriter.InitConfigFile(AppDomain.CurrentDomain.BaseDirectory + "\\Log4netConfigFile.xml");
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            string command = "1";
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["IsWindowsService"]))
            {
                command = System.Configuration.ConfigurationManager.AppSettings["IsWindowsService"];
            }
            if (command.Trim() == "0")
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("正在初始化服务器。。。");
                XletterCallBackManagment.GetManagment().Init();
                Console.WriteLine("服务器已启动。。。");
                command = Console.ReadLine();
                XletterCallBackManagment.GetManagment().Stop();
            }
            else
            {
                try
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] { new XlettlerCallBackService() };
                    ServiceBase.Run(ServicesToRun);
                }
                catch (Exception ex)
                {
                    LogTool.LogWriter.WriteError("未捕获运行异常。", ex);
                }
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogTool.LogWriter.WriteError("未捕获异常。", (Exception)e.ExceptionObject);
        }
    }
}
