using ChannelInterFace;
using ChannelManagement;
using Model;
using PublicDefined;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    SESENT_Order order = dbm.SESENT_Order.Where(a => a.OrderID == OrderID).FirstOrDefault();
                    if (order == null) { LogTool.LogWriter.WriteError($"订单不存在!【订单号:{OrderID}】"); return false; }
                    SESENT_Channels channel = dbm.SESENT_Channels.Where(a => a.ChannelID == ChannelID).FirstOrDefault();
                    if (channel == null) { LogTool.LogWriter.WriteError($"通道编号不存在:通道编号:{ChannelID}订单编号:{OrderID}"); return false; }
                    TPayRechargeBase payRecharge = ChannelProtocolManage.GetManagment().GetChannelProtocol(channel.ProtocolID);
                    if (payRecharge == null) { LogTool.LogWriter.WriteError($"通道协议错误!"); return false; }
                    bool ret = payRecharge.Notify(OrderID, pathArges, UrlArges, postBuffer, out BackStr);
                    if (ret) order.Status = (short)OrderStatus.success;
                    dbm.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    return dbm.SaveChanges() > 0;
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
