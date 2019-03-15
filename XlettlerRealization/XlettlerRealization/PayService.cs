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
        public bool MakeOrder(decimal amount, string accountID, string channelID, out MakeOrderNewData result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            if (amount <= 0) { errMsg = "充值金额错误!"; return false; }
            using (ModelContainer container = new ModelContainer())
            {
                SESENT_Order order = new SESENT_Order(){AccountID = accountID,InputMoney = amount,ChannelID = channelID,OrderID = RuleGenerateOrder.GetOrderID(),OrderType = (short)OrderType.AccountRecharge,Status = (short)OrderStatus.fail, OrderTime=DateTime.Now};
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
                    bool ret = thrid.MakeOrder(amount, order.OrderTime, out result, out errMsg);
                    if (ret) { order.Status = (short)OrderStatus.wait; }
                    return true;
                }
                catch (Exception err)
                {
                    errMsg = "下单失败!";
                    LogTool.LogWriter.WriteError($"{accountID}账户下单失败!", err);
                    return false;
                }
                finally {
                    container.SESENT_Order.Add(order);
                    container.SaveChanges();
                }
            }
        }

        public IList<SESENT_Channels> QueryChannels()
        {
            using (ModelContainer container=new ModelContainer()) 
            {
                try
                {
                   return container.SESENT_Channels.Where(a => a.Status == (short)Status.Open).ToList();
                }
                catch (Exception err)
                {
                    LogTool.LogWriter.WriteError("通道查询错误!",err);
                    return null;
                }
            }
        }

        public IList<SESENT_Order> QueryOrder(string accountID, DateTime OrderTime,int type,int pageIndex,int pageSize,out string errMsg)
        {
            errMsg = string.Empty;
            using (ModelContainer container = new ModelContainer()) 
            {
               SESENT_USERS uSERS=container.SESENT_USERS.Where(a => a.AccountID == accountID||a.Phone==accountID).FirstOrDefault();
                try
                {
                    if (uSERS == null) { errMsg = "查询账户不存在!";return null;}
                    return container.SESENT_Order.Where(a=>a.AccountID==accountID&&a.OrderTime==OrderTime&&a.OrderType==type).Skip(pageIndex*pageSize).Take(pageSize).ToList();
                }
                catch (Exception err)
                {
                    LogTool.LogWriter.WriteError($"查询订单接口失败!【账号:{accountID}查询时间:{OrderTime.ToString("yyyy-MM-dd HH:mm:ss")}】",err);
                    return null;
                }
            }
        }
    }
}
