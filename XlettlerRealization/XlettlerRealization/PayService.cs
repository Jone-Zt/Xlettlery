using ChannelInterface.ChannelInterface;
using ChannelInterFace;
using ChannelManagement;
using Model;
using ModelComparer;
using PublicDefined;
using RuleUtility;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace XlettlerRealization
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class PayService : IPayInterface
    {
        public bool BindCashBank(SESENT_CashCard cashCard,out string result,out string errMsg)
        {
            result = null;
            errMsg =string.Empty;
            try
            {
                using (ModelContainer db = new ModelContainer())
                {
                    SESENT_USERS uSERS=db.SESENT_USERS.Where(a => a.AccountID == cashCard.AccountID).FirstOrDefault();
                    if (uSERS == null) { errMsg = "绑定用户不存在!";return false; }
                    SESENT_CashCard bankCard=db.SESENT_CashCard.Where(a => a.BankNumber == cashCard.BankNumber && a.AccountID == cashCard.AccountID).FirstOrDefault();
                    if (bankCard != null) { errMsg = "该用户已绑定该银行卡";return false;}
                    db.SESENT_CashCard.Add(cashCard);
                    bool ret=db.SaveChanges()>0;
                    if (!ret)
                        errMsg = "绑定失败!";
                    else
                        result = "绑定成功!";
                    return ret;
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError($"绑定银行卡失败:{cashCard}", err);
                return false;
            }
        }

        public Dictionary<string, object> CashWithdrawal(string accountID, decimal amount, int bankID)
        {
            //10001:未绑定真实姓名 10002:未绑定手机号 10003:一般错误  10004:提现正在受理中 10005:未绑定提现银行卡
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (amount <= 0) {
                result.Add("Status", "10003");
                result.Add("Info","提现金额错误!");
                return result;
            }
            using (ModelContainer db = new ModelContainer())
            {
                try
                {
                    SESENT_USERS uSERS = db.SESENT_USERS.Where(a => a.AccountID == accountID).FirstOrDefault();
                    if (uSERS == null) {
                        result.Add("Status", "10003");
                        result.Add("Info", "账户不存在!");
                    }
                    if (string.IsNullOrEmpty(uSERS.RealName)) { result.Add("Status", "10001"); result.Add("Info", "未绑定真实姓名"); return result; }
                    if (string.IsNullOrEmpty(uSERS.Phone)) {  result.Add("Status", "10002");  result.Add("Info", "未绑定手机号!"); return result; }
                    SESENT_CashCard cashCard = db.SESENT_CashCard.Where(a => a.AccountID == accountID && a.Id == bankID).FirstOrDefault();
                    if (cashCard == null) {
                        result.Add("Status", "10005");
                        result.Add("Info", "未绑定提现银行卡!");
                        return result;
                         }
                    //查询当前用户可提现的金额
                    IQueryable<SESENT_ConsumptERecords> queryData = from val in db.SESENT_ConsumptERecords
                                                                    join order in db.SESENT_Order
                                                                    on val.OrderID equals order.OrderID
                                                                    where order.Status == (short)OrderStatus.wait
                                                                    && val.AccountID == accountID && val.OutMoney == order.OutMoney
                                                                    orderby val.EntryTime
                                                                    select val;
                    SESENT_ConsumptERecords records = queryData.FirstOrDefault();
                    if (records != null)
                    {
                        if (records.Withdrawable <= amount)
                        {
                            if (records.Withdrawable == amount)
                                records.Status = (short)OrderStatus.success;
                            records.Withdrawable -= amount;
                            //生成订单
                            db.SESENT_Order.Add(new SESENT_Order()
                            {
                                AccountID = accountID,
                                BankID = cashCard.Id,
                                ChannelID = "账户提现",
                                InputMoney = amount,
                                OrderID = RuleUtility.RuleGenerateOrder.GetOrderID(),
                                OrderTime = DateTime.Now,
                                OrderType = (short)OrderType.CashWithdrow,
                                Status = (short)OrderStatus.wait,
                            });
                            db.SESENT_ConsumptERecords.Add(records);
                            db.Entry(records).State = System.Data.Entity.EntityState.Modified;
                            bool ret = db.SaveChanges() > 0;
                            if (ret)
                            {
                                result.Add("Status", "10004");
                                result.Add("Info", "提现正在受理中");
                                return result;
                            }
                            else
                            {
                                result.Add("Status", "10003");
                                result.Add("Info", "提现处理失败");
                                return result;
                            }
                        }
                        else
                        {
                            result.Add("Status", "10003");
                            result.Add("Info", "提现金额不足!");
                            return result;
                        }
                    }
                    else
                    {
                        result.Add("Status", "10003");
                        result.Add("Info", "未查询到可提现订单!");
                        return result;
                    }
                }
                catch (Exception err)
                {
                    result.Add("Status", "10003");
                    result.Add("Info", "提现失败!");
                    LogTool.LogWriter.WriteError($"{accountID}账户提现失败!", err);
                    return result;
                }
            }
        }

        public bool MakeOrder(decimal amount, string accountID, string channelID, out MakeOrderNewData result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            if (amount <= 0) { errMsg = "充值金额错误!"; return false; }
            using (ModelContainer container = new ModelContainer())
            {
                SESENT_Order order = new SESENT_Order() { AccountID = accountID, InputMoney = amount, ChannelID = channelID, OrderID = RuleGenerateOrder.GetOrderID(), OrderType = (short)OrderType.AccountRecharge, Status = (short)OrderStatus.fail, OrderTime = DateTime.Now };
                try
                {
                    SESENT_USERS uSERS = container.SESENT_USERS.Where(a => a.AccountID == accountID).FirstOrDefault();
                    if (uSERS == null) { errMsg = "充值账户不存在!"; return false; }
                    //通道去查询
                    short channelStatus = (short)Status.Open;
                    SESENT_Channels channel = container.SESENT_Channels.Where(a => a.ChannelID == channelID && a.Status == channelStatus).FirstOrDefault();
                    if (channel == null) { errMsg = "通道暂未开放!"; LogTool.LogWriter.WriteError($"订单下单通道查询失败【通道编号:{channelID}】"); return false; }
                    TPayRechargeBase payRecharge = ChannelProtocolManage.GetManagment().GetChannelProtocol(channel.ProtocolID);
                    if (payRecharge == null) { errMsg = "未找到通道文件"; return false; }
                    IThridRecharge thrid = payRecharge as IThridRecharge;
                    if (thrid == null) { errMsg = "通道未实现"; return false; }
                    bool ret = thrid.MakeOrder(channelID,order.OrderID,amount, order.OrderTime, out result, out errMsg);
                    if (ret) { order.Status = (short)OrderStatus.wait; }
                    return true;
                }
                catch (Exception err)
                {
                    order.Status = (short)OrderStatus.fail;
                    order.FailReason = "下单失败!";
                    errMsg = "下单失败!";
                    LogTool.LogWriter.WriteError($"{accountID}账户下单失败!", err);
                    return false;
                }
                finally
                {
                    container.SESENT_Order.Add(order);
                    container.SaveChanges();
                }
            }
        }

        public bool QueryBankName(out IList<string> result, out string errMsg)
        {
            errMsg = string.Empty;
            result = null;
            try
            {
                using (ModelContainer db = new ModelContainer())
                {
                    result= db.SESENT_BankLineNumber.Select(a => a.BankName).Distinct().ToList();
                    return true;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("查询支持银行名称失败", err);
                return false;
            }
        }

        public bool QueryCashBank(string accountID,out IList<SESENT_CashCard> result , out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer db = new ModelContainer())
                {
                    SESENT_USERS uSERS=db.SESENT_USERS.Where(a => a.AccountID == accountID).FirstOrDefault();
                    if (uSERS == null) { errMsg = "查询用户不存在!"; return false;}
                    result=db.SESENT_CashCard.Where(a => a.AccountID == accountID).OrderBy(a=>a.EnterTime).ToList();
                    return true;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError($"查询提现银行卡失败!账号:{accountID}", err);
                return false;
            }
        }

        public IList<SESENT_Channels> QueryChannels()
        {
            using (ModelContainer container = new ModelContainer())
            {
                try
                {
                    return container.SESENT_Channels.Where(a => a.Status == (short)Status.Open).ToList();
                }
                catch (Exception err)
                {
                    LogTool.LogWriter.WriteError("通道查询错误!", err);
                    return null;
                }
            }
        }

        public bool QueryNetSite(string Province, string City, string BankName,string NetSite, out SESENT_BankLineNumber[] result,out string errMsg)
        {
            errMsg = string.Empty;
            result = null;
            using (ModelContainer container = new ModelContainer())
            {
                var customers = (from c in container.SESENT_BankCity
                                where c.Province.Contains(Province)&&c.City.Contains(City) 
                                select c).FirstOrDefault();
                result = container.SESENT_BankLineNumber.Where(a => a.Provinceid == customers.ID && a.NetSite.Contains(NetSite) && (string.IsNullOrEmpty(BankName) ? a.BankName == BankName : true)).ToArray();
                return true;
            }
        }

        public QueryOrderPicker QueryOrder(string accountID, DateTime BeginTime,DateTime EndTime, int type, int pageIndex, int pageSize, out string errMsg)
        {
            errMsg = string.Empty;
            QueryOrderPicker picker = new QueryOrderPicker();
            using (ModelContainer container = new ModelContainer())
            {
                SESENT_USERS uSERS = container.SESENT_USERS.Where(a => a.AccountID == accountID || a.Phone == accountID).FirstOrDefault();
                try
                {
                    EndTime = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, 23, 59, 59);
                    BeginTime = new DateTime(BeginTime.Year,BeginTime.Month,BeginTime.Day,0,0,0);
                    if (uSERS == null) { errMsg = "查询账户不存在!"; return null; }
                    picker.Orders=container.SESENT_Order.Where(a => a.AccountID == accountID && (a.OrderTime >= BeginTime||a.OrderTime<=EndTime)  && a.OrderType == type).OrderByDescending(b=>b.OrderTime).Skip((pageIndex-1) * pageSize).Take(pageSize).ToList();
                    int count = container.SESENT_Order.Where(a => a.AccountID == accountID && (a.OrderTime >= BeginTime || a.OrderTime <= EndTime) && a.OrderType == type).Count();
                    int totalPage = (count + pageSize - 1) / pageSize;
                    picker.PageNums = totalPage;
                    picker.PageSize = count;
                    return picker;
                }
                catch (Exception err)
                {
                    LogTool.LogWriter.WriteError($"查询订单接口失败!【账号:{accountID}查询时间:{BeginTime.ToString("yyyy-MM-dd HH:mm:ss")}】", err);
                    errMsg = "未知错误！";
                    return null;
                }
            }
        }
    }
}
