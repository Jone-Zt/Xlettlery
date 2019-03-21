using Models;
using PublicDefined;
using ServicesInterface;
using System;
using System.Web.Mvc;
using Tools;

namespace MobileAPI.Controllers
{
    public class UserController : Controller
    {
        private IUserInterface GetManger()
        {
            IUserInterface proxy = null;
            if (proxy == null)
            {
                proxy = RemotingAngency.GetRemoting().GetProxy<IUserInterface>();
            }
            return proxy;
        }
        public ActionResult Register()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string AccountID = RequestCheck.CheckStringValue(Request, "AccountID", "账号", false);
                string passWord = RequestCheck.CheckStringValue(Request, "passWord", "密码", false);
                string Phone = RequestCheck.CheckStringValue(Request, "Phone", "手机号", false);
                string Code = RequestCheck.CheckStringValue(Request, "Code", "手机验证码", false);
                string agencyID = RequestCheck.CheckStringValue(Request, "agencyID", "代理编号", false);
                UserType userType = UserType.member;
                IUserInterface proxy = GetManger();
                if (proxy == null)
                    throw new Exception("未挂载函数");
                if (!proxy.Register(AccountID, passWord, Phone, agencyID, Code, userType, out string errMsg))
                    picker.FailInfo = errMsg;
                else
                    picker.Data = "注册成功!";
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult QueryUserInfo()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string AccountID = RequestCheck.CheckStringValue(Request, "AccountID","账号/手机号",false);
                IUserInterface user = GetManger();
                if (user == null)
                    throw new Exception("未挂载函数!");
                bool ret=user.QueryUserInfo(AccountID, out object result, out string errMsg);
                if (ret)
                    picker.Data = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult Login()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string AccountID = RequestCheck.CheckStringValue(Request, "AccountID", "账号", true);
                string passWord = RequestCheck.CheckStringValue(Request, "passWord", "密码", true);
                string Phone = RequestCheck.CheckStringValue(Request, "Phone", "手机号", true);
                string Code = RequestCheck.CheckStringValue(Request, "Code", "手机验证码", true);
                int? type = RequestCheck.CheckIntValue(Request, "Type", "登陆类型", false);
                IUserInterface user = GetManger();
                if (user == null)
                    throw new Exception("未挂载函数!");
                if (user.Login(AccountID, passWord, Phone, Code, (LoginType)type, out string errMsg))
                    picker.Data = "登陆成功!";
                else
                    picker.FailInfo = "登陆失败!";
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult FindLoginPwd()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string Phone = RequestCheck.CheckStringValue(Request, "Phone", "手机号", false);
                string Code = RequestCheck.CheckStringValue(Request, "Code", "手机验证码", false);
                string passWord = RequestCheck.CheckStringValue(Request, "passWord", "修改密码", false);
                IUserInterface user = GetManger();
                if (user == null)
                    throw new Exception("未挂载函数!");
                bool ret = user.FindLoginPwd(Phone, Code, passWord, out string errMsg);
                if (ret)
                    picker.Data = "修改成功!";
                else
                    picker.FailInfo = "修改失败,请联系客服。";
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult SendUserCode()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string Phone = RequestCheck.CheckStringValue(Request, "Phone", "手机号", false);
                int? type = RequestCheck.CheckIntValue(Request, "type", "短信类型", false);
                IPhoneCodeType phoneCode = (IPhoneCodeType)type;
                IUserInterface user = GetManger();
                if (user == null)
                    throw new Exception("未挂载函数!");
                bool ret = user.SendUserCode(Phone, phoneCode);
                if (ret)
                    picker.Data = "发送成功!";
                else
                    picker.FailInfo = "发送失败!";
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
    }
}