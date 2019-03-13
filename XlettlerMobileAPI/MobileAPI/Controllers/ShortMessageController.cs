using Models;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                string accountID = RequestCheck.CheckStringValue(Request,"AccountID","账号/手机号",false);
                picker.FlowID = flowid;
                IShortMessageInterface user = GetManger();
                if (user == null)
                    throw new Exception("未挂载函数!");
                if (user.QueryInbox(accountID, out IList<object> result, out string errMsg))
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
    }
}