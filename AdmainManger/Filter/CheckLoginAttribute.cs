using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AdmainManger.Filter
{
    public class CheckLoginAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            using (System.IO.Stream  stream=filterContext.HttpContext.Request.InputStream) {
                StringBuilder builder = new StringBuilder();
                if (stream.CanRead) {
                    byte[] bts = new byte[1024];
                    builder.Append("请求参数:");
                    while (stream.Read(bts,0,bts.Length)!=0)
                    {
                        builder.Append(Encoding.UTF8.GetString(bts).Trim());
                        bts = new byte[1024];
                    }
                }
                LogTool.LogWriter.WriteDebug(builder.ToString());
            }
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false)) { return; }
            if (filterContext.ActionDescriptor.IsDefined(typeof(SkipCheckLoginAttribute), false)){return;}
            if (filterContext.HttpContext.Session["User"] == null){ filterContext.HttpContext.Response.Redirect("/Login/Index");}
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (System.IO.Stream stream = filterContext.HttpContext.Response.OutputStream)
            {
                StringBuilder builder = new StringBuilder();
                if (stream.CanRead)
                {
                    byte[] bts = new byte[1024];
                    builder.Append("响应参数:");
                    while (stream.Read(bts, 0, bts.Length) != 0)
                    {
                        builder.Append(Encoding.UTF8.GetString(bts).Trim());
                        bts = new byte[1024];
                    }
                }
                LogTool.LogWriter.WriteDebug(builder.ToString());
            }
        }
    }
}