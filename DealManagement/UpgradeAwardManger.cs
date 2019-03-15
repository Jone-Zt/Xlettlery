using Model;
using PublicDefined;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DealManagement
{
    /// <summary>
    /// 奖励管理
    /// </summary>
    public class UpgradeAwardManger
    {
        private UpgradeAwardManger() { }
        private static UpgradeAwardManger Managment;
        private static object lockObj = new object();
        public static UpgradeAwardManger GetManagment()
        {
            if (Managment == null)
            {
                lock (lockObj)
                {
                    Interlocked.CompareExchange(ref Managment, new UpgradeAwardManger(), null);
                }
            }
            return Managment;
        }
        public bool SendShortMessage(string AccountID, MessageObjType message, string Title, string Content, bool HasGift, decimal Gift,ModelContainer db)
        {
            try
            {
                SESENT_USERS uSERS = null;
                    if (message == MessageObjType.Singler)
                    {
                       uSERS = db.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                        if (uSERS == null) throw new Exception("发送的对象不存在!");
                    }
                    db.SESENT_MessageText.Add(new SESENT_MessageText()
                    {
                        Content = Content,
                        CreateDateTime = DateTime.Now,
                        Gift = HasGift ? Gift : 0,
                        HasGift = HasGift,
                        Recld = uSERS==null?"-1":uSERS.AccountID,
                        SendID = "admin",
                        Title = Title,
                        Type = (short)message
                    });
                   return db.SaveChanges() > 0;
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError($"发送站内信失败:【账号:{AccountID},messageType:{message},Gift:{Gift}】", err);
                return false;
            }
        }
    }
}
