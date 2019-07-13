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
            //在出现未处理的错误时运行的代码         
            Exception objError = Server.GetLastError().GetBaseException();
            string errortime = string.Empty;
            string erroraddr = string.Empty;
            string errorinfo = string.Empty;
            string errorsource = string.Empty;
            string errortrace = string.Empty;
            string errormethodname = string.Empty;
            string errorclassname = string.Empty;
            errortime = "发生时间:" + System.DateTime.Now.ToString();
            erroraddr = "发生异常页: " + System.Web.HttpContext.Current.Request.Url.ToString();
            errorinfo = "异常信息: " + objError.Message;
            errorsource = "错误源:" + objError.Source;
            errortrace = "堆栈信息:" + objError.StackTrace;
            errorclassname = "发生错误的类名" + objError.TargetSite.DeclaringType.FullName;
            errormethodname = "发生错误的方法名：" + objError.TargetSite.Name;
            if (objError.Message.Contains("上未找到公共操作方法"))
                Response.StatusCode = 404;
            Server.ClearError();
            string ip = "用户IP:" + Request.UserHostAddress;
            string log = errortime + "##" + erroraddr + "##" + ip + "##" + errorclassname + "##" + errormethodname + "##" + errorinfo + "##" + errorsource + "##" + errortrace.Replace("\r\n", "<br>");
            LogTool.LogWriter.WriteError(this.GetType().Name, new Exception(log));
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
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

    }
}
