using ChannelManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace XlettleryTimerTaskService
{
    partial class XletteryTimerTackService : ServiceBase
    {
        public XletteryTimerTackService()
        {
            InitializeComponent();
            this.CanPauseAndContinue = false;
        }

        protected override void OnStart(string[] args)
        {
            TimingTaskManage.GetManagment().Init();
        }

        protected override void OnStop()
        {
            TimingTaskManage.GetManagment().Stop();
        }
    }
}
