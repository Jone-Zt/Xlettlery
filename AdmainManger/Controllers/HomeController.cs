using Model;
using PublicDefined;
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
            Dictionary<int, string> OrderTypes = new Dictionary<int, string>();
            OrderTypes.Add((int)OrderType.AccountConsumption,"账户消费");
            OrderTypes.Add((int)OrderType.AccountRecharge, "账户充值");
            OrderTypes.Add((int)OrderType.CashWithdrow, "账户提现");
            OrderTypes.Add((int)OrderType.GradeAward, "等级奖励");
            ViewBag.OrderTypes = OrderTypes;
            return View(); ;
        }
        [HttpPost]
        public ActionResult QueryOrder(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("key",((OrderType)id).ToString());
            using (ModelContainer db = new ModelContainer())
            {
               IQueryable<SESENT_Order> Orders=db.SESENT_Order.Where(a => a.OrderType == id);
                var item = Orders.GetEnumerator();
                while (item.MoveNext())
                    item.Current.ChannelID= db.SESENT_Channels.Where(a => a.ChannelID == item.Current.ChannelID).FirstOrDefault()?.ChannelName?? item.Current.ChannelID;
               dic.Add("value",Orders.ToList());
            }
            return Json(dic);
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