using ChannelManagement.TimerJobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChannelManagement
{
    public class TimingTaskManage
    {
        private static TimingTaskManage Managment;
        private static object lockObj = new object();
        private IScheduler scheduler;
        ISchedulerFactory factory;
        public static TimingTaskManage GetManagment()
        {
            if (Managment == null)
            {
                lock (lockObj)
                {
                    Interlocked.CompareExchange(ref Managment, new TimingTaskManage(), null);
                }
            }
            return Managment;
        }
        public void Init()
        {
            factory = new StdSchedulerFactory();
            scheduler = factory.GetScheduler();
            AddJob();
            scheduler.Start();
        }
        public void Stop()
        {
            if (scheduler != null)
            {
                scheduler.Shutdown(true);
            }
        }
        private void AddJob()
        {
            IJobDetail job = JobBuilder.Create<AutomaticCalculation>().WithIdentity("job", "group").Build();
            ITrigger trigger = TriggerBuilder.Create().WithIdentity("trigger", "group").WithCronSchedule("0/5 * * * * ?").Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}
