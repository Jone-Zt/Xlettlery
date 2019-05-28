using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobileAPI.Controllers
{
    public class ErrorCodeController : Controller
    {
        public ActionResult Unauthorized()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            picker.FailInfo = "API未经授权";
            picker.MsgCode = (int)ResType.Unauthorized;
            return Content(picker.ToString());
        }
        public ActionResult NoFind()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            picker.FailInfo = "请求路径错误,请仔细核对";
            picker.MsgCode = (int)ResType.NoFind;
            return Content(picker.ToString());
        }
    }
}