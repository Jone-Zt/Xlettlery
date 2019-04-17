using Models;
using PublicDefined;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tools;

namespace MobileAPI.Controllers
{
    public class ShortMessageController : Controller
    {
        private IShortMessageInterface proxy;
        private IShortMessageInterface GetManger()
        {
            if (proxy == null)
            {
                proxy = RemotingAngency.GetRemoting().GetProxy<IShortMessageInterface>();
            }
            return proxy;
        }
        public ActionResult QueryInbox()
        {
            ResponsePicker<Dictionary<string, string>> picker = new ResponsePicker<Dictionary<string, string>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                string accountID = RequestCheck.CheckStringValue(Request, "AccountID", "账号/手机号", false);
                picker.FlowID = flowid;
                IShortMessageInterface user = GetManger();
                if (user == null)
                    throw new Exception("未挂载函数!");
                if (user.QueryInbox(accountID, out IList<Dictionary<string, string>> result, out string errMsg))
                    picker.List = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult InboxOperation()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                string accountID = RequestCheck.CheckStringValue(Request, "AccountID", "账号", false);
                string messageID = RequestCheck.CheckStringValue(Request, "messageID", "邮箱编号", false);
                int? OperationType = RequestCheck.CheckIntValue(Request, "OperationType", "操作类型", false);
                picker.FlowID = flowid;
                IShortMessageInterface user = GetManger();
                if (user == null)
                    throw new Exception("未挂载函数!");
                if (user.InboxOperation(accountID, messageID, (MessageStatus)OperationType, out string errMsg))
                    picker.Data = "操作成功!";
                else
                    picker.FailInfo = string.IsNullOrEmpty(errMsg) ? "操作失败!" : errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
    }
}