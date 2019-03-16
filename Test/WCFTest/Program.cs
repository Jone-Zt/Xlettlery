using ChannelManagement.TimerJobs;
using System;
using System.Threading;
using Tools;

namespace WCFTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var channelFactory = new ChannelFactory<IUserInterface>(new NetTcpBinding(), "net.tcp://localhost:44556/IUserInterface"))
            //{
            //    //手动创建代理类
            //    var proxy = channelFactory.CreateChannel();
            //    //Console.WriteLine(proxy.Login("zhangteng","dsadsa"));
            //}
            //ActiveMQHelper.GetManger().SendMessage("15510555669", PublicDefined.IPhoneCodeType.Login);
            //Thread.Sleep(30000);
            //var result=RedisHelper.GetManger().Get(CacheKey.GenerateCachePhoneCode("15510555669", PublicDefined.IPhoneCodeType.Login));
            AutomaticCalculation automaticCalculation = new AutomaticCalculation();
            automaticCalculation.Execute(null);
            Console.ReadLine();
        }
    }
}
