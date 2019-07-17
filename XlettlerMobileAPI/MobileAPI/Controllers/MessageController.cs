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
    public class MessageController : Controller
    {
        private IMessageInterface GetManger()
        {
            IMessageInterface proxy = null;
            if (proxy == null)
            {
                proxy = RemotingAngency.GetRemoting().GetProxy<IMessageInterface>();
            }
            return proxy;
        }
        public ActionResult QueryInformations()
        {
            ResponsePicker<Model.SESENT_InfoMation> picker = new ResponsePicker<Model.SESENT_InfoMation>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? ID = RequestCheck.CheckIntValue(Request, "ID", "资讯编号", false);
                IMessageInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                if (pay.QueryInformation((int)ID,out Model.SESENT_InfoMation result, out string errMsg))
                    picker.Data = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = (err as MyException)?.outErrMsg ?? err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult QueryInformationWithList()
        {
            ResponsePicker<Dictionary<string, object>> picker = new ResponsePicker<Dictionary<string, object>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                IMessageInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                if (pay.QueryInformationWithList(out IList<Dictionary<string, object>> result, out string errMsg))
                    picker.List = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = (err as MyException)?.outErrMsg ?? err.Message;
            }
            return Content(picker.ToString());
        }
    }
}