using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MobileAPI.Controllers
{
    public class WxChatController : Controller
    {
        public ActionResult WxChatNotify()
        {
            if (!string.IsNullOrEmpty(Request["echostr"]))
                return Content(Request["echostr"]);
            else
                Response.ContentType = "text/xml";
            using (Stream stm = Request.InputStream)
            {
                StreamReader Reader = new StreamReader(stm, Encoding.UTF8);
                string NotifyStr = Reader.ReadToEnd();
                LogTool.LogWriter.WriteDebug($"微信回调信息:{NotifyStr}");
            }
            return null;
        }
    }
}