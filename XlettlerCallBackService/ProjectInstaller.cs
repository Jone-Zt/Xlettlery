﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace XlettlerCallBackService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceInstaller1.Description = "迷彩平台 支付回调服务，回调服务都依赖该服务。";
            this.serviceInstaller1.DisplayName = "XlettlerCallBackService";
        }
    }
}
