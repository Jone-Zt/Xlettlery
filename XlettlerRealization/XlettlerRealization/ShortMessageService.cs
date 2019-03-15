using Model;
using PublicDefined;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XlettlerRealization
{
    public class ShortMessageService : IShortMessageInterface
    {
        public bool InboxOperation(string AccountID, string MessageID, MessageStatus OperationType, out string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                using (ModelContainer db = new ModelContainer())
                {
                    if (OperationType == MessageStatus.Collected || OperationType == MessageStatus.Readed)
                    {
                        if (int.TryParse(MessageID, out int msgID))
                        {
                            SESENT_MessageText messageText = db.SESENT_MessageText.Where(a => a.Id == msgID).FirstOrDefault();
                            if (messageText == null) { errMsg = "未持有该信息!"; return false; }
                            if (messageText.Type == (short)MessageObjType.Singler)
                            {
                                if (messageText.Recld != AccountID) { errMsg = "信息匹配失败!"; return false; }
                            }
                            SESENT_USERS uSERS = db.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                            if (uSERS == null) { errMsg = "账户不存在!"; return false; }
                            SESENT_MessageObj obj = db.SESENT_MessageObj.Where(a => a.MessageID == msgID && a.RecId == AccountID).FirstOrDefault();
                            if (obj != null && (obj.Status == (short)MessageStatus.Readed || obj.Status == (short)MessageStatus.Collected)) { errMsg = "信息已处理!"; return false; }
                            if (OperationType == MessageStatus.Readed)
                            {
                                if (obj == null)
                                {
                                    obj = new SESENT_MessageObj() { MessageID = msgID, RecId = AccountID, ReciveDateTime = DateTime.Now, Status = (short)MessageStatus.Readed };
                                    db.SESENT_MessageObj.Add(obj); return db.SaveChanges() > 0;
                                }
                                else
                                {
                                    obj.Status = (short)MessageStatus.Readed; db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                                }
                            }
                            else
                            {
                                if (!messageText.HasGift) { errMsg = "邮箱未存在可领取奖励!";return false; }
                                if (obj == null) {
                                    obj = new SESENT_MessageObj() { MessageID = msgID, RecId = AccountID, ReciveDateTime = DateTime.Now, Status = (short)MessageStatus.Collected };
                                    db.SESENT_MessageObj.Add(obj);
                                }  else
                                {
                                    obj.Status = (short)MessageStatus.Readed;
                                    db.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                                }
                                db.SESENT_Order.Add(new SESENT_Order()
                                {
                                    AccountID = AccountID,
                                    ChannelID = "等级奖励",
                                    OrderID = RuleUtility.RuleGenerateOrder.GetOrderID(),
                                    InputMoney = messageText.Gift,
                                    OrderTime = DateTime.Now,
                                    OrderType = (short)OrderType.GradeAward,
                                    OutMoney = messageText.Gift,
                                    Status = (short)OrderStatus.success
                                });
                                uSERS.UseAmount += messageText.Gift;
                                db.SESENT_USERS.Add(uSERS);
                                db.Entry(uSERS).State = System.Data.Entity.EntityState.Unchanged;
                            }
                            db.SaveChanges();
                            return true;
                        }
                        else
                        {
                            errMsg = "消息编号错误!";
                            return false;
                        }
                    }
                    else
                    {
                        errMsg = "操作类型错误!";
                        return false;
                    }
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError($"收件箱操作失败 【账号：{AccountID}操作类型:{OperationType}】", err);
                return false;
            }
        }

        public bool QueryInbox(string AccountID, out IList<Dictionary<string, string>> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = new List<Dictionary<string, string>>();
                    IQueryable<SESENT_MessageText> texts = container.SESENT_MessageText.Where(a => a.Type == (short)MessageObjType.All || (a.Type == (short)MessageObjType.Singler && a.Recld == AccountID));
                    var item = texts.GetEnumerator();
                    while (item.MoveNext())
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        SESENT_MessageObj obj = container.SESENT_MessageObj.Where(a => a.MessageID == item.Current.Id && a.RecId == AccountID).FirstOrDefault();
                        dic.Add("ID", item.Current.Id.ToString());
                        if (obj != null)
                            dic.Add("Status", obj.Status.ToString());
                        else
                            dic.Add("Status", ((int)MessageStatus.Unreaded).ToString());
                        dic.Add("Title", item.Current.Title);
                        dic.Add("Content", item.Current.Content);
                        dic.Add("HasGift", (item.Current.HasGift ? 1 : 0).ToString());
                        if (item.Current.HasGift)
                            dic.Add("Gift", item.Current.Gift.ToString("0"));
                        dic.Add("CreateTime", item.Current.CreateDateTime.ToString("yyyy-MM-dd"));
                        dic.Add("SendID", item.Current.SendID);
                        result.Add(dic);
                    }
                }
                return true;
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError($"查询收件箱 【账号：{AccountID}】", err);
                return false;
            }
        }
    }
}
