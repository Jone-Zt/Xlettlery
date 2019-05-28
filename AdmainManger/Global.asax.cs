using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AdmainManger
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LogTool.LogWriter.InitConfigFile(AppDomain.CurrentDomain.BaseDirectory + "\\Log4netConfigFile.xml");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
