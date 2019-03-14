using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace XlettlerCallBackService
{
    partial class XlettlerCallBackService : ServiceBase
    {
        public XlettlerCallBackService()
        {
            InitializeComponent();
            this.CanPauseAndContinue = false;
        }

        protected override void OnStart(string[] args)
        {
            XletterCallBackManagment.GetManagment().Init();
        }

        protected override void OnStop()
        {
            XletterCallBackManagment.GetManagment().Stop();
        }
    }
}
