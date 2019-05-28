using Model;
using Models;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Tools;

namespace MobileAPI.Controllers
{
    [Authorize]
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
                LogTool.LogWriter.WriteError("支付客服端挂载失败!", err);
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
                string accountID = RequestCheck.CheckStringValue(Request, "accountID", "账号", false);
                string channelID = RequestCheck.CheckStringValue(Request, "channelID", "通道编号", false);
                decimal amount = RequestCheck.CheckDecimalValue(Request, "amount", "金额", false);
                IPayInterface pay = GetManger();
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
        public ActionResult QueryChannels()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                IPayInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                IList<SESENT_Channels> list = pay.QueryChannels();
                if (list == null || list.Count == 0)
                    picker.FailInfo = "暂无支付通道信息!";
                else
                    picker.Data = list;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult QueryOrder()
        {
            ResponsePicker<SESENT_Order> picker = new ResponsePicker<SESENT_Order>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string accountID = RequestCheck.CheckStringValue(Request, "accountID","账号",false);
                DateTime OrderTime = RequestCheck.CheckDeteTimeValue(Request, "OrderTime","订单时间",false);
                int? PageIndex = RequestCheck.CheckIntValue(Request, "pageIndex","页数",false);
                int? Type = RequestCheck.CheckIntValue(Request,"type","订单类型",false);
                int? PageSize = RequestCheck.CheckIntValue(Request, "pageSize","每页总量",false);
                IPayInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                IList<SESENT_Order> list = pay.QueryOrder(accountID, OrderTime, Convert.ToInt32(Type), Convert.ToInt32(PageIndex), Convert.ToInt32(PageSize), out string errMsg);
                if (list == null)
                    picker.FailInfo = errMsg;
                else
                    picker.List = list;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
    }
}