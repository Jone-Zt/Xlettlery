using AdmainManger.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdmainManger.Controllers
{
    [SkipCheckLogin]
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}