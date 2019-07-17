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
    public class AwardOpeningController : Controller
    {
        private IAwardOpeningService GetManger()
        {
            IAwardOpeningService proxy = null;
            if (proxy == null)
            {
                proxy = RemotingAngency.GetRemoting().GetProxy<IAwardOpeningService>();
            }
            return proxy;
        }
        public ActionResult QueryFootBallAward()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                DateTime dateTime = RequestCheck.CheckDeteTimeValue(Request,"queryTime","查询时间",false);
                IAwardOpeningService pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                if (pay.GetFootBallAward(dateTime,out dynamic result, out string errMsg))
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
    }
}