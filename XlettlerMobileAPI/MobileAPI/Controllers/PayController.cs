using Models;
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
            try
            {
                if (proxy == null)
                {
                    proxy = RemotingAngency.GetRemoting().GetProxy<IPayInterface>();
                }
                return proxy;
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError("支付客服端挂载失败!",err);
                return null;
            }
        }
        /// <summary>
        ///下单 
        /// </summary>
        public ActionResult MakeOrder()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string accountID = RequestCheck.CheckStringValue(Request, "accountID","账号",false);
                string channelID = RequestCheck.CheckStringValue(Request, "channelID","通道编号",false);
                decimal amount = RequestCheck.CheckDecimalValue(Request, "amount","金额",false);
                IPayInterface pay=GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                if (pay.MakeOrder(amount, accountID, channelID, out MakeOrderNewData result, out string errMsg))
                {
                    if (result != null)
                    {
                        if (result.IsHtml)
                            return Content(result.Result);
                        else
                            return Redirect(result.Result);
                    }
                    else
                        return Content("通道响应失败!");
                }
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
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