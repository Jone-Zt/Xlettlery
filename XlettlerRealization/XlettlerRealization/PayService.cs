using ChannelInterface.ChannelInterface;
using ChannelInterFace;
using ChannelManagement;
using Model;
using PublicDefined;
using RuleUtility;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XlettlerRealization
{
    public class PayService : IPayInterface
    {
        public bool BindCashBank(SESENT_CashCard cashCard, out string errMsg)
        {
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
                    return ret;
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError($"绑定银行卡失败:{cashCard}", err);
                return false;
            }
        }

        public bool CashWithdrawal(string accountID, decimal amount, int bankID, out Dictionary<string, object> result)
        {
            result = new Dictionary<string, object>();
            result.Add("Status", (int)CashWithdrawalProcess.CashNoneFind);
            if (amount <= 0) { result.Add("errMsg", "提现金额错误!"); return false; }
            using (ModelContainer db = new ModelContainer())
            {
                try
                {
                    SESENT_USERS uSERS = db.SESENT_USERS.Where(a => a.AccountID == accountID).FirstOrDefault();
                    if (uSERS == null) { result.Add("errMsg", "账户不存在!"); return false; }
                    if (string.IsNullOrEmpty(uSERS.RealName) || string.IsNullOrEmpty(uSERS.Phone))
                    {
                        result["Status"] = (int)CashWithdrawalProcess.NoBindRealName; result.Add("errMsg", "未绑定提现信息!"); return false;
                    }
                    SESENT_CashCard cashCard = db.SESENT_CashCard.Where(a => a.AccountID == accountID && a.Id == bankID).FirstOrDefault();
                    if (cashCard == null) { result["Status"] = (int)CashWithdrawalProcess.NoBindBankCard; result.Add("errMsg", "未绑定提现银行卡!"); return false; }
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
                                result.Add("Status", (int)OrderStatus.success);
                            else
                                result.Add("Status", (int)OrderStatus.fail);
                            return ret;
                        }
                        else
                        {
                            result.Add("errMsg", "提现金额不足!");
                            return false;
                        }
                    }
                    else
                    {
                        result.Add("errMsg", "未查询到可提现订单!");
                        return false;
                    }
                }
                catch (Exception err)
                {
                    result.Add("errMsg", "提现失败!");
                    LogTool.LogWriter.WriteError($"{accountID}账户提现失败!", err);
                    return false;
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

        public IList<SESENT_CashCard> QueryCashBank(string accountID, out string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                using (ModelContainer db = new ModelContainer())
                {
                    SESENT_USERS uSERS=db.SESENT_USERS.Where(a => a.AccountID == accountID).FirstOrDefault();
                    if (uSERS == null) { errMsg = "查询用户不存在!"; return null;}
                    return db.SESENT_CashCard.Where(a => a.AccountID == accountID).OrderBy(a=>a.EnterTime).ToList();
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError($"查询提现银行卡失败!账号:{accountID}", err);
                return null;
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

        public Dictionary<string,object> QueryOrder(string accountID, DateTime OrderTime, int type, int pageIndex, int pageSize, out string errMsg)
        {
            errMsg = string.Empty;
            Dictionary<string, object> result = new Dictionary<string, object>();
            using (ModelContainer container = new ModelContainer())
            {
                SESENT_USERS uSERS = container.SESENT_USERS.Where(a => a.AccountID == accountID || a.Phone == accountID).FirstOrDefault();
                try
                {
                    if (uSERS == null) { errMsg = "查询账户不存在!"; return null; }
                    result.Add("Data",container.SESENT_Order.Where(a => a.AccountID == accountID && a.OrderTime == OrderTime && a.OrderType == type).Skip(pageIndex * pageSize).Take(pageSize).ToList());
                    int count = container.SESENT_Order.Where(a => a.AccountID == accountID && a.OrderTime == OrderTime && a.OrderType == type).Count();
                    int totalPage = (count + pageSize - 1) / pageSize;
                    result.Add("totlePages", totalPage);
                    result.Add("totleCount",count);
                    return result;
                }
                catch (Exception err)
                {
                    LogTool.LogWriter.WriteError($"查询订单接口失败!【账号:{accountID}查询时间:{OrderTime.ToString("yyyy-MM-dd HH:mm:ss")}】", err);
                    return null;
                }
            }
        }
    }
}
