using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace XlettlerService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
                LogTool.LogWriter.InitConfigFile(AppDomain.CurrentDomain.BaseDirectory + "\\Log4netConfigFile.xml");
                LogTool.LogWriter.WriteInfo("通道服务开始启动中...");
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
                string command = "1";
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["IsWindowsService"]))
                {
                    command = System.Configuration.ConfigurationManager.AppSettings["IsWindowsService"];
                }
                if (command.Trim() == "0")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("正在初始化服务器。。。");
                    XletterManagment.GetManagment().Init();
                    Console.ReadLine();
                    XletterManagment.GetManagment().Stop();
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] { new XlettlerService() };
                    ServiceBase.Run(ServicesToRun);
                }
            }
            catch (Exception ex2)
            {
                LogTool.LogWriter.WriteError("通道服务启动失败。", ex2);
                throw;
            }
        }
    }
}
