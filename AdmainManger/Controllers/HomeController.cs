using Model;
<<<<<<< HEAD
using Models;
=======
>>>>>>> de088fc9e5a13d704bd455d8f638318f2835b11d
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
                return Content(MessageBox.Show("请选择上传文件!"));
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
                    if (ret) return Content(MessageBox.Show("操作成功!"));
                    else return Content(MessageBox.Show("操作失败!"));
                    }
            }
            else {
                return Content(MessageBox.Show("请选择上传文件类型!"));
            }
        }
    }
}