using AdmainManger.Filter;
using Model;
using Models;
using PublicDefined;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tools;

namespace AdmainManger.Controllers
{
    [CheckLogin]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OrderManger()
        {
            Dictionary<int, string> OrderTypes = new Dictionary<int, string>();
            OrderTypes.Add((int)OrderType.AccountRecharge, "账户充值");
            OrderTypes.Add((int)OrderType.AccountConsumption, "账户消费");
            OrderTypes.Add((int)OrderType.GradeAward, "等级奖励");
            OrderTypes.Add((int)OrderType.CashWithdrow, "账户提现");
            ViewBag.OrderTypes = OrderTypes;
            return View(); ;
        }
        public ActionResult OptionChannel(string id)
        {
            using (ModelContainer db = new ModelContainer()) 
            {
                SESENT_Channels sESENT= db.SESENT_Channels.Where(a => a.ChannelID == id).FirstOrDefault();
                if (sESENT == null) return Content("通道不存在!");
                bool ret=sESENT.Status == (short)Status.Open;
                if (ret)
                    sESENT.Status = (short)Status.Close;
                else
                    sESENT.Status = (short)Status.Open;
                db.SESENT_Channels.Add(sESENT);
                db.Entry(sESENT).State = System.Data.Entity.EntityState.Modified;
                ret=db.SaveChanges()>0;
                if (ret) return Content("操作成功!");
                else return Content("操作失败!");
            }
        }
        [HttpPost]
        public ActionResult QueryOrder(int id)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            using (ModelContainer db = new ModelContainer())
            {
               IQueryable<SESENT_Order> Orders=db.SESENT_Order.Where(a => a.OrderType == id);
                string UserName = Request["userName"];
                if (!string.IsNullOrEmpty(UserName))
                    Orders=Orders.Where(a => a.AccountID == UserName);
                string beginTime = Request["beginTime"];
                string endTime = Request["endTime"];
                if (!string.IsNullOrEmpty(beginTime)&&DateTime.TryParse(beginTime,out DateTime parseBeginTime))
                    Orders = Orders.Where(a => a.OrderTime >= parseBeginTime);
                if (!string.IsNullOrEmpty(endTime) && DateTime.TryParse(endTime, out DateTime parseEndTime))
                    Orders = Orders.Where(a => a.OrderTime<= parseEndTime);
               Orders=Orders.OrderByDescending(a => a.OrderTime);
                int count = Orders.Count();
                dic.Add("size", count);
                string pageIndex = Request["pageIndex"];
                string pageSize = Request["pageSize"];
                if (!string.IsNullOrEmpty(pageIndex) && int.TryParse(pageIndex, out int ParsePageIndex) && !string.IsNullOrEmpty(pageSize) && int.TryParse(pageSize, out int ParsePageSize))
                {
                    Orders = Orders.Skip((ParsePageIndex - 1) * ParsePageSize).Take(ParsePageSize);
                }
                var item = Orders.GetEnumerator();
                while (item.MoveNext())
                    item.Current.ChannelID= db.SESENT_Channels.Where(a => a.ChannelID == item.Current.ChannelID).FirstOrDefault()?.ChannelName?? item.Current.ChannelID;
                dic.Add("value",Orders.ToList());
            }
            return Json(dic);
        }
        public ActionResult QueryDetail(string id)
        {
            Dictionary<string, object> div = null;
            if (!string.IsNullOrEmpty(id)) {
                using (ModelContainer db = new ModelContainer())
                {
                    SESENT_Order Order = db.SESENT_Order.Where(a => a.OrderID == id).FirstOrDefault();
                    div = UntilsObjToDic.ToMap(Order);
                    if (Order != null) 
                        div["ChannelID"] =db.SESENT_Channels.Where(a => a.ChannelID == Order.ChannelID).FirstOrDefault()?.ChannelName ?? Order.ChannelID;
                    if (Order != null && Order.OrderType == (short)OrderType.CashWithdrow&&int.TryParse(Order?.BankID.ToString(),out int Bank)) {
                        SESENT_CashCard cashCard=db.SESENT_CashCard.Where(a => a.Id == Bank).FirstOrDefault();
                        div["BankID"] = cashCard.BankNumber;
                    }
                }
            }
            return Json(div);
        }
        public ActionResult Console()
        {
            return View();
        }
        public ActionResult ChannelManger()
        {
            return View();
        }
        public ActionResult QueryChannels()
        {
            using (ModelContainer db = new ModelContainer())
            {
                IQueryable<SESENT_Channels> channels=db.SESENT_Channels.Where(a => true);
                var item=channels.GetEnumerator();
                while (item.MoveNext())
                {
                   bool ret=db.SESENT_ChannelProtocol.Where(a => a.ProtocolID == item.Current.ProtocolID).Count()>0;
                    if (!ret)
                        item.Current.ProtocolID = "协议编号错误";
                }
                return Json(channels.ToList());
            }
        }
        public ActionResult Setting()
        {
            using (ModelContainer db = new ModelContainer())
            {
               List<SESENT_Settings> ImgList = db.SESENT_Settings.Where(a => true).ToList();
               ViewBag.ImgList = ImgList;
               ViewBag.OpenStatus =(short)Status.Open;
               ViewBag.Type = (short)SettingType.Advertisement;
                Dictionary<short, string> dic = new Dictionary<short, string>();
                dic.Add((short)SettingType.Advertisement,"轮播图");
                dic.Add((short)SettingType.Guide, "引导图");
                ViewBag.UpdateType = dic;
            }
            return View();
        }
        public ActionResult DeleteImg(int id)
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
           string remask=Request["remask"];
            using (ModelContainer db = new ModelContainer())
            {
                SESENT_Settings settings = db.SESENT_Settings.Where(a => a.Id == id).FirstOrDefault();
                if (settings == null) { picker.FailInfo = "未查询到信息!"; return Content(picker.ToString()); }
                if (remask == "delete")
                {
                    if (settings.Status == (short)Status.Open) { picker.FailInfo = "当前配置信息正在使用中。"; return Content(picker.ToString()); }
                    db.SESENT_Settings.Add(settings);
                    db.Entry(settings).State = System.Data.Entity.EntityState.Deleted;
                }
                else if (remask == "remask") {
                    string type=Request["type"];
                    short realType = -1;
                    if (string.IsNullOrEmpty(type)||!short.TryParse(type,out realType)) { picker.FailInfo = "标记类型错误!"; }
                    Status remaskType=(Status)realType;
                    settings.Status = (short)realType;
                    db.SESENT_Settings.Add(settings);
                    db.Entry(settings).State = System.Data.Entity.EntityState.Modified;
                }
                bool ret = db.SaveChanges() > 0;
                if (ret) { picker.Data = "操作成功!"; }
                else picker.FailInfo = "操作失败!";
            }
            return Content(picker.ToString());
        }
        public ActionResult UpFiles()
        {
            HttpPostedFileBase file=Request.Files["UploadFile"];
            if (file == null||file.ContentLength==0) 
                return Content("请选择上传文件!");
            Image pic = Image.FromStream(file.InputStream);
            short type = -1;
            if (short.TryParse(Request["type"], out type))
            {
                MemoryStream memoryStream = new MemoryStream();
                using (Stream stm = file.InputStream)
                {
                    file.InputStream.Seek(0, SeekOrigin.Begin);
                    file.InputStream.CopyTo(memoryStream);
                }
                    int width = pic.Width;
                    int height = pic.Height;
                    using (ModelContainer db = new ModelContainer())
                    {
                        db.SESENT_Settings.Add(new SESENT_Settings() {
                            Height = (short)height,
                            Width = (short)width,
                            Type = type,
                            image = Convert.ToBase64String(memoryStream.ToArray()),
                            Status = (short)Status.Close,
                        });
                     bool ret=db.SaveChanges()>0;
                    if (ret) return Content("操作成功!");
                    else return Content("操作失败!");
                    }
            }
            else {
                return Content("请选择上传文件类型!");
            }
        }
    }
}