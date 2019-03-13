using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace XlettlerService
{
    partial class XlettlerService : ServiceBase
    {
        public XlettlerService()
        {
            InitializeComponent();
            this.CanPauseAndContinue = false;
        }

        protected override void OnStart(string[] args)
        {
            XletterManagment.GetManagment().Init();
        }

        protected override void OnStop()
        {
            XletterManagment.GetManagment().Stop();
        }
    }
}
