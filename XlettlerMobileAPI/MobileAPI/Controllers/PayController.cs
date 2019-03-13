using ServicesInterface;
using System;
using System.IO;
using System.Web.Mvc;
using Tools;

namespace MobileAPI.Controllers
{
    public class PayController : Controller
    {
        private IPayInterface proxy;
        private IPayInterface GetManger()
        {
            if (proxy == null)
            {
                proxy = RemotingAngency.GetRemoting().GetProxy<IPayInterface>();
            }
            return proxy;
        }
        /// <summary>
        ///下单 
        /// </summary>
        public ActionResult MakeOrder()
        {
            return null;
        }
        /// <summary>
        /// 通道查询
        /// </summary>
        public ActionResult QueryChannel()
        {
            return null;
        }
    }
}