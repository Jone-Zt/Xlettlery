using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tools;

namespace AdmainManger.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OrderManger()
        {
            return View(); ;
        }
        public ActionResult Console()
        {
            return View();
        }
        public ActionResult ChannelManger()
        {
            return View();
        }
        public ActionResult Setting()
        {
            return View();
        }
    }
}