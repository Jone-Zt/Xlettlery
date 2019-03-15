using DealManagement;
using Model;
using PublicDefined;
using Quartz;
using System;
using System.Linq;
using System.Transactions;

namespace ChannelManagement.TimerJobs
{
    public class AutomaticCalculation : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Model.ModelContainer db = new Model.ModelContainer();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //检测所有用户上次升级时间
                    DateTime UpgradeTime = DateTime.Now.AddDays(-90);
                    IQueryable<SESENT_USERS> uSERs = db.SESENT_USERS.Where(a => a.Lv > 0 && a.UpgradeTime<= UpgradeTime);
                    var item = uSERs.GetEnumerator();
                    while (item.MoveNext())
                    {
                        //查询用户流水
                        short status = (short)OrderType.AccountConsumption;
                        short orderStatus = (short)OrderStatus.success;
                        SESENT_RankingSystemSetting setting = null;
                        int count = db.SESENT_Order.Where(a => a.AccountID == item.Current.AccountID && a.OrderType == status && a.OrderTime >= UpgradeTime && a.OrderTime <= DateTime.Now && a.OrderType == orderStatus).Count();
                        if (count <= 0)
                        {
                            //降级处理
                            item.Current.Lv --;
                            if (item.Current.Lv == 0) {  item.Current.Consumption = 0; item.Current.Recharge = 0;
                            } else{ setting = db.SESENT_RankingSystemSetting.Where(a => a.Lv == item.Current.Lv).FirstOrDefault();
                              item.Current.Consumption = setting.Consumption; item.Current.Recharge = setting.Recharge;
                            }
                        }
                        //更新检测时间
                        item.Current.UpgradeTime = DateTime.Now;
                        db.SESENT_USERS.Add(item.Current);
                        db.Entry<SESENT_USERS>(item.Current).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        setting=db.SESENT_RankingSystemSetting.Where(a => a.Lv == item.Current.Lv).FirstOrDefault();
                        if (setting != null)
                        UpgradeAwardManger.GetManagment().SendShortMessage(item.Current.AccountID, MessageObjType.Singler, "等级奖励", "尊贵的会员,小小心意、请老板笑纳！", true, setting.Reward,db);
                    }
                    scope.Complete();
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError("定时任务处理失败!",err);
            }
        }
    }
}
