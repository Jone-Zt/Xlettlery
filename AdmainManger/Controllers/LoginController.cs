using AdmainManger.Filter;
using Model;
using SecurityTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tools;

namespace AdmainManger.Controllers
{
    [SkipCheckLogin]
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login()
        {
            string userName = Request["userName"];
            string userPwd = Request["userPwd"];
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPwd))
                return Content(MessageBox.Show("账号密码不可为空."));
            using (ModelContainer db = new ModelContainer())
            {
                string pwd = ShaEncryption.Encryption(userPwd);
                SESENT_AdminManger adminManger=db.SESENT_AdminManger.Where(a=>a.userName==userName&&a.userPwd==pwd).FirstOrDefault();
                if (adminManger == null) return Content(MessageBox.Show("账号/密码错误!"));
                Session["User"] = adminManger.userName;
                adminManger.LoginLastTime = DateTime.Now;
                db.SESENT_AdminManger.Add(adminManger);
                bool ret=db.SaveChanges()>0;
                if (ret) return RedirectToAction("Index", "Home");
                else return RedirectToAction("Index");
            }
        }
    }
}