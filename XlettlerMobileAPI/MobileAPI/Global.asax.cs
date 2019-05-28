using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MobileAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            LogTool.LogWriter.InitConfigFile(AppDomain.CurrentDomain.BaseDirectory + "\\Log4netConfigFile.xml");
        }
        protected void Application_Error(object sender, EventArgs e)
        {
           
        }
        protected void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Response.StatusCode == 401)
                Response.Redirect("/ErrorCode/Unauthorized");
        }
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Response.StatusCode == 404)
                Response.Redirect("/ErrorCode/NoFind");
        }

    }
}
