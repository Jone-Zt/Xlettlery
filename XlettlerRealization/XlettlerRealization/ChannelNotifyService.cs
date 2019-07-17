using ChannelInterFace;
using ChannelManagement;
using Model;
using PublicDefined;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace XlettlerRealization
{
    public class ChannelNotifyService : IChannelNotify
    {
        public bool Notify(string OrderID, string ChannelID, string[] pathArges, Dictionary<string, string> UrlArges, byte[] postBuffer, out string BackStr)
        {
            BackStr = string.Empty;
            try
            {
                using (ModelContainer dbm = new ModelContainer())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        SESENT_Order order = dbm.SESENT_Order.Where(a => a.OrderID == OrderID).FirstOrDefault();
                        if (order == null) { LogTool.LogWriter.WriteError($"订单不存在!【订单号:{OrderID}】");  return false; }
                        SESENT_Channels channel = dbm.SESENT_Channels.Where(a => a.ChannelID == ChannelID).FirstOrDefault();
                        if (channel == null) { LogTool.LogWriter.WriteError($"通道编号不存在:通道编号:{ChannelID}订单编号:{OrderID}"); return false; }
                        SESENT_USERS _USERS=dbm.SESENT_USERS.Where(a => a.AccountID == order.AccountID || a.Phone == order.AccountID).FirstOrDefault();
                        if (_USERS == null) { LogTool.LogWriter.WriteError($"【订单回调失败】账号:{order.AccountID}订单号:{order.OrderID}"); return false; }
                        TPayRechargeBase payRecharge = ChannelProtocolManage.GetManagment().GetChannelProtocol(channel.ProtocolID);
                        if (payRecharge == null) { LogTool.LogWriter.WriteError($"通道协议错误!"); return false; }
                        bool ret = payRecharge.Notify(OrderID, pathArges, UrlArges, postBuffer, out BackStr,out decimal realPay);
                        if (ret)
                        {
                            order.Status = (short)OrderStatus.success;
                            order.OutMoney = realPay == 0 ? order.InputMoney : realPay;
                            _USERS.UseAmount += order.OutMoney;
                            _USERS.Recharge += order.OutMoney;
                        }
                        dbm.Entry(order).State = System.Data.Entity.EntityState.Modified;
                        dbm.Entry(_USERS).State = System.Data.Entity.EntityState.Modified;
                        bool isSuccess = dbm.SaveChanges() > 0;
                        transaction.Complete();
                        return isSuccess;
                    }
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError("回调错误:", err);
                return false;
            }
        }
    }
}
