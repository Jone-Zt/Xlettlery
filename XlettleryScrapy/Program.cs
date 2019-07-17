using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using XlettleryScrapy.LqHandler;
using XlettleryScrapy.ZqHandler;

namespace XlettleryScrapy
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
            LogTool.LogWriter.InitConfigFile(AppDomain.CurrentDomain.BaseDirectory + "\\Log4netConfigFile.xml");
            LogTool.LogWriter.WriteInfo("爬虫服务开始启动中...");
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
            while (true)
            {
                Console.WriteLine("请输入爬取命令!");
                string cmd = Console.ReadLine();
                switch (cmd.ToLower())
                {
                    case "lq":
                        LqListScrapyHandler.GetHandler("https://trade.500.com/jclq/index.php?playid=313&g=2");
                        break;
                    case "zq":
                        ZqListScrapyHandler.GetHandler("https://trade.500.com/jczq/?playid=312&g=2");
                        break;
                    case "zqkj":
                        Console.WriteLine("请输入抓取时间!");
                        string time = Console.ReadLine();
                        DateTime Parsetime = DateTime.Parse(time);
                        ZqKjScrapyHandler.GetHandler("http://zx.500.com/jczq/kaijiang.php?playid=0&d=", Parsetime);
                        break;
                }
            }
        }
    }
}
