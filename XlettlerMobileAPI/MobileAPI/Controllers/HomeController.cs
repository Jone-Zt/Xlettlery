using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tools;

namespace MobileAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            if (Request.HttpMethod == "GET")
            {
                return View();
            }
            else {
                string password = Request["password"]?.Trim();
                string username = Request["username"]?.Trim();
                return Content(MessageBox.Show("账号错误!"));
            }
        }
        public ActionResult UpdateLoginPwd()
        {
            return Content("忘记密码了？");
        }
    }
}
