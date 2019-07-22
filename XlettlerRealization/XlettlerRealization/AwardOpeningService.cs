using AOPHandlerManager.MethordHandler;
using Model;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace XlettlerRealization
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class AwardOpeningService : IAwardOpeningService
    {
        [MethordTimingHandler]
        public bool GetFootBallAward(DateTime QueryTime,out List<dynamic> picker,out string errMsg)
        {
            errMsg = string.Empty;
            picker = new List<dynamic>();
            try
            {
                using (Model.ModelContainer container = new Model.ModelContainer())
                {
                    QueryTime = DateTime.Parse(QueryTime.ToString("yyyy-MM-dd 0:0:0"));
                    DateTime endTime = DateTime.Parse(QueryTime.ToString("yyyy-MM-dd 23:59:59"));
                    IQueryable<IGrouping<string,SESENT_KJLottery>> kJLotteries=container.SESENT_KJLottery.Where(a => a.LotteryTime == QueryTime&&a.GameType==(int)PublicDefined.LetteryType.FootBall).GroupBy(a=>a.No);
                    foreach (IGrouping<string,SESENT_KJLottery> item in kJLotteries)
                    {
                        //根据当前时间查询到他的主场编号
                        SESENT_FootBallMatch match =container.SESENT_FootBallMatch.Where(a => a.MatchDate>=QueryTime&&a.MatchDate<= endTime&& a.No == item.Key).FirstOrDefault();
                        if (match == null)  { errMsg = "未查询到该场赛次编号!";return false; }
                        if (match != null)
                        {
                            picker.Add(new
                            {
                                match,
                                item
                            });
                        }
                    }
                    return true;
                }
            }
            catch(Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("查询足球开奖接口:"+err);
                return false;
            }
        }
    }
}
