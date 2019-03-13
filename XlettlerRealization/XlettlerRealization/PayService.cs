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
        public bool MakeOrder(decimal amount, string accountID, out string payUrl,out string errMsg)
        {
            errMsg = string.Empty;
            payUrl = string.Empty;
            if (amount <= 0) { errMsg = "充值金额错误!"; return false; }
            using (ModelContainer container = new ModelContainer()) 
            {
                try
                {
                    SESENT_USERS uSERS=container.SESENT_USERS.Where(a => a.AccountID == accountID).FirstOrDefault();
                    if (uSERS == null) { errMsg = "充值账户不存在!";return false;}
                    container.SESENT_Order.Add(new SESENT_Order()
                    {
                          AccountID=accountID,
                          InputMoney=amount,
                          OrderID= RuleGenerateOrder.GetOrderID(),
                          OrderType=(short)OrderType.AccountRecharge,
                          Status= (short)OrderStatus.wait,
                    });
                    bool ret = container.SaveChanges() > 0;
                    if (!ret) { errMsg = "订单创建失败!";return false; }
                    //通道去查询



                    return true;
                }
                catch (Exception err)
                {
                    errMsg = "下单失败!";
                    LogTool.LogWriter.WriteError($"{accountID}账户下单失败!",err);
                    return false;
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

        public IList<SESENT_Order> QueryOrder(string accountID, DateTime OrderTime,int pageIndex,int pageSize,out string errMsg)
        {
            errMsg = string.Empty;
            using (ModelContainer container = new ModelContainer()) 
            {
               SESENT_USERS uSERS=container.SESENT_USERS.Where(a => a.AccountID == accountID||a.Phone==accountID).FirstOrDefault();
                try
                {
                    if (uSERS == null) { errMsg = "查询账户不存在!";return null;}
                    return container.SESENT_Order.Where(a=>a.AccountID==accountID&&a.OrderTime==OrderTime).Skip(pageIndex*pageSize).Take(pageSize).ToList();
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
