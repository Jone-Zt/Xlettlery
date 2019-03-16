using ChannelManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace XlettleryTimerTaskService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
                LogTool.LogWriter.InitConfigFile(AppDomain.CurrentDomain.BaseDirectory + "\\Log4netConfigFile.xml");
                LogTool.LogWriter.WriteInfo("定时服务开始启动中...");
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
                    TimingTaskManage.GetManagment().Init();
                    Console.ReadLine();
                    TimingTaskManage.GetManagment().Stop();
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] { new XletteryTimerTackService() };
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
