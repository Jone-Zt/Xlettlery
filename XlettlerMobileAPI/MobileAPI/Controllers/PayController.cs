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
                picker.FailInfo = (err as MyException)?.outErrMsg ?? err.Message;
            }
            return Content(picker.ToString());
        }

        public ActionResult QueryCashBank()
        {
            ResponsePicker<SESENT_CashCard> picker = new ResponsePicker<SESENT_CashCard>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string accountID = RequestCheck.CheckStringValue(Request, "accountID", "账号", false);
                IPayInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                if (pay.QueryCashBank(accountID, out IList<SESENT_CashCard> result, out string errMsg))
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
        public ActionResult CashWithdrawal()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string accountID = RequestCheck.CheckStringValue(Request, "accountID", "账号", false);
                decimal amount = RequestCheck.CheckDecimalValue(Request, "amount", "金额", false);
                int? BankID = RequestCheck.CheckIntValue(Request, "BankID", "绑定编号", false);
                IPayInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                picker.Data = pay.CashWithdrawal(accountID, amount, int.Parse(BankID.ToString()));
            }
            catch (Exception err)
            {
                picker.FailInfo = (err as MyException)?.outErrMsg ?? err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult BindCashBank()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string AccountID = RequestCheck.CheckStringValue(Request, "AccountID", "用户编号", false);
                string NetSite= RequestCheck.CheckStringValue(Request, "NetSite", "开户行网点", false);
                string BankCode = RequestCheck.CheckStringValue(Request, "BankCode", "银行编码", false);
                string BankName = RequestCheck.CheckStringValue(Request, "BankName", "银行名称", false);
                string BankNumber = RequestCheck.CheckStringValue(Request, "BankNumber", "银行卡号", false);
                string LineNumber = RequestCheck.CheckStringValue(Request, "BankNumber", "联行号", false);
                IPayInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                SESENT_CashCard cashCard = new SESENT_CashCard()
                {
                    AccountID = AccountID,
                    NetSite = NetSite,
                    BankCode = BankCode,
                    BankName = BankName,
                    BankNumber = BankNumber,
                    EnterTime = DateTime.Now,
                    LineNumber= LineNumber
                };
                if (pay.BindCashBank(cashCard, out string result, out string errMsg))
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

    /// <summary>
    /// 通道查询
    /// </summary>
    //[Authorize]
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
                picker.FailInfo = (err as MyException)?.outErrMsg ?? err.Message;
            }
            return Content(picker.ToString());
        }
        //[Authorize]
        public ActionResult QueryOrder()
        {
            ResponsePicker<QueryOrderPicker> picker = new ResponsePicker<QueryOrderPicker>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string accountID = RequestCheck.CheckStringValue(Request, "accountID","账号",false);
                DateTime OrderTime = RequestCheck.CheckDeteTimeValue(Request, "BeginTime","开始时间",false);
                DateTime EndTime = RequestCheck.CheckDeteTimeValue(Request, "EndTime", "结束时间", false);
                int? PageIndex = RequestCheck.CheckIntValue(Request, "pageIndex","页数",false);
                int? Type = RequestCheck.CheckIntValue(Request,"type","订单类型",false);
                int? PageSize = RequestCheck.CheckIntValue(Request, "pageSize","每页总量",false);
                IPayInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                QueryOrderPicker data = pay.QueryOrder(accountID, OrderTime, EndTime, Convert.ToInt32(Type), Convert.ToInt32(PageIndex), Convert.ToInt32(PageSize), out string errMsg);
                if (data == null)
                    picker.FailInfo = errMsg;
                else
                    picker.Data = data;
            }
            catch (Exception err)
            {
                picker.FailInfo = (err as MyException)?.outErrMsg ?? err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult QueryNetSite()
        {
            ResponsePicker<SESENT_BankLineNumber> picker = new ResponsePicker<SESENT_BankLineNumber>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                string Province = RequestCheck.CheckStringValue(Request, "Province", "省份", false);
                string City = RequestCheck.CheckStringValue(Request, "City", "城市", false);
                string BankName = RequestCheck.CheckStringValue(Request, "BankName", "银行名称", false);
                string NetSite = RequestCheck.CheckStringValue(Request, "NetSite", "开户网点(模糊搜索)", false);
                IPayInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                bool isSuccess = pay.QueryNetSite(Province, City, BankName,NetSite,out SESENT_BankLineNumber[] result,out string errMsg);
                if (result == null)
                    picker.FailInfo = "未知错误!";
                else
                    picker.List = result;
            }
            catch (Exception err)
            {
                picker.FailInfo = (err as MyException)?.outErrMsg ?? err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult QueryBankName()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                IPayInterface pay = GetManger();
                if (pay == null) throw new Exception("未挂载对应函数!");
                if (pay.QueryBankName(out IList<string> bankNames, out string errMsg))
                    picker.List = (IList<object>)bankNames;
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