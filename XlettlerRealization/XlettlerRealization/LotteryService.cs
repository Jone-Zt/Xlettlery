using DealManagement;
using Model;
using RuleUtility;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace XlettlerRealization
{
    public class LotteryService : IlotteryInterface
    {
        public bool QueryBasketBallLottery(int lotteryId, out List<MySlefGeneratePicker<SESENT_BasketBallGame, SESENT_BasketBallMatch>> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = new List<MySlefGeneratePicker<SESENT_BasketBallGame, SESENT_BasketBallMatch>>();
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open && a.Type == (int)PublicDefined.LetteryType.BasketBall).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    DateTime dataNow = DateTime.Now;
                    for (int i = 0; i < 3; i++)
                    {
                        dataNow = DateTime.Parse(dataNow.ToString("yyyy-MM-dd 0:0:0"));
                        List<Model.SESENT_BasketBallMatch> list = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_BasketBallMatch>();
                        IEnumerable<SESENT_BasketBallMatch> matches = list.Where(a => a.MatchDate == dataNow);
                        var item = matches.GetEnumerator();
                        while (item.MoveNext())
                        {
                            MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch> picker = new MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch>();
                            picker.Match = item.Current;
                            List<Model.SESENT_BasketBallGame> games = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_BasketBallGame>();
                            var letBall = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.Letball);
                            picker.BallGames.Add((int)PublicDefined.LqGageType.Letball, letBall.ToList());
                            var doubleResult = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.SizeSoure);
                            picker.BallGames.Add((int)PublicDefined.LqGageType.SizeSoure, doubleResult.ToList());
                            var NumberofGoalsScored = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.VictoryOrFail);
                            picker.BallGames.Add((int)PublicDefined.LqGageType.VictoryOrFail, NumberofGoalsScored.ToList());
                            var Score = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.VictoryOrFailDiff_Main);
                            picker.BallGames.Add((int)PublicDefined.LqGageType.VictoryOrFailDiff_Main, Score.ToList());
                            var VictoryOrFailDiff_Visable = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.VictoryOrFailDiff_Visable);
                            picker.BallGames.Add((int)PublicDefined.LqGageType.VictoryOrFailDiff_Visable, VictoryOrFailDiff_Visable.ToList());
                            result.Add(picker);
                        }
                        dataNow.AddDays(1);
                    }
                    return true;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("CP查询支持接口:", err);
                return false;
            }
        }
        public bool QueryFootBallLotteryWithType(int lotteryId, long FootBallID, int? type, out MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = new MySlefGeneratePicker<SESENT_FootBallGame, SESENT_FootBallMatch>();
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open && a.Type == (int)PublicDefined.LetteryType.FootBall).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    SESENT_FootBallMatch footBallMatch = container.SESENT_FootBallMatch.Where(a => a.FootballID == FootBallID).FirstOrDefault();
                    if (footBallMatch == null) { errMsg = "未查询到该场次!"; return false; }
                    double timeSpan = Tools.RedisHelper.GetManger().GetTimeOut(Tools.CacheKey.GenerateCacheGameBall<SESENT_FootBallMatch>(footBallMatch.Fk_FnID.ToString())).Value.TotalMinutes;
                    if (timeSpan <= 10)
                    { errMsg = "赛事开奖前10分钟内停止投注"; return false; }
                    else
                    {
                        if (type != null)
                        {
                            PublicDefined.ZqGameType lqGametype = (PublicDefined.ZqGameType)type;
                            Dictionary<int, List<SESENT_FootBallGame>> keyValuePairs = new Dictionary<int, List<SESENT_FootBallGame>>();
                            keyValuePairs.Add((int)lqGametype, Tools.RedisHelper.GetManger().GetWithList<SESENT_FootBallGame>().Where(Z => Z.FootballID == FootBallID && Z.Type == type).ToList());
                            result.BallGames = keyValuePairs;
                            result.Match = footBallMatch;
                        }
                        else
                        {
                           IEnumerable<SESENT_FootBallGame> ballGames=Tools.RedisHelper.GetManger().GetWithList<SESENT_FootBallGame>().Where(Z => Z.FootballID == FootBallID);
                            Dictionary<int, List<SESENT_FootBallGame>> keyValuePairs = new Dictionary<int, List<SESENT_FootBallGame>>();
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.Letball, ballGames.Where(Z => Z.Type ==(int)PublicDefined.ZqGameType.Letball).ToList());
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.DoubleResult, ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.DoubleResult).ToList());
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.LetBallWithSigler, ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.LetBallWithSigler).ToList());
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.NotLatball, ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.NotLatball).ToList());
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.NotLatBallWithSigler, ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.NotLatBallWithSigler).ToList());
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.NumberofGoalsScored, ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.NumberofGoalsScored).ToList());
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.Score, ballGames.Where(Z =>  Z.Type == (int)PublicDefined.ZqGameType.Score).ToList());
                            result.BallGames = keyValuePairs;
                            result.Match = footBallMatch;
                        }
                        return true;
                    }
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("CP查询支持接口:", err);
                return false;
            }
        }

        public bool QueryFootBallLottery(int lotteryId, out List<MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch>> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = new List<MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch>>();
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open && a.Type == (int)PublicDefined.LetteryType.FootBall).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    DateTime dataNow = DateTime.Now;
                    for (int i = 0; i < 3; i++)
                    {
                        //查询当前的赛事
                        List<Model.SESENT_FootBallMatch> list = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_FootBallMatch>();
                        IEnumerable<SESENT_FootBallMatch> matches = list.Where(a => a.MatchDate.ToString("yyyy-MM-dd") == dataNow.ToString("yyyy-MM-dd"));
                        var item = matches.GetEnumerator();
                        while (item.MoveNext())
                        {
                            MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch> picker = new MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch>();
                            picker.Match = item.Current;
                            List<Model.SESENT_FootBallGame> games = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_FootBallGame>();
                            //默认的两种玩法
                            var letBall = games.Where(a => a.FootballID == item.Current.FootballID && a.Type == (int)PublicDefined.ZqGameType.Letball);
                            picker.BallGames.Add((int)PublicDefined.ZqGameType.Letball, letBall.ToList());
                            var NotletBall = games.Where(a => a.FootballID == item.Current.FootballID && a.Type == (int)PublicDefined.ZqGameType.NotLatball);
                            picker.BallGames.Add((int)PublicDefined.ZqGameType.NotLatball, NotletBall.ToList());
                            result.Add(picker);
                        }
                        dataNow = dataNow.AddDays(1);
                    }
                    return true;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("CP查询支持接口:", err);
                return false;
            }
        }

        public bool QuerySuportLottery(out IList<SESENT_Lottery> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = container.SESENT_Lottery.Where(a => true).ToList();
                    return true;
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError("CP查询支持接口:", err);
                return false;
            }
        }

        public bool QueryBasketBallLotteryWithType(int lotteryId, long BaskBallID, int type, out MySlefGeneratePicker<SESENT_BasketBallGame, SESENT_BasketBallMatch> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = new MySlefGeneratePicker<SESENT_BasketBallGame, SESENT_BasketBallMatch>();
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open && a.Type == (int)PublicDefined.LetteryType.BasketBall).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    SESENT_BasketBallMatch basketBallMatch = container.SESENT_BasketBallMatch.Where(a => a.BasketballID == BaskBallID).FirstOrDefault();
                    if (basketBallMatch == null) { errMsg = "未查询到该场次!"; return false; }
                    double timeSpan = Tools.RedisHelper.GetManger().GetTimeOut(Tools.CacheKey.GenerateCacheGameBall<SESENT_BasketBallMatch>(basketBallMatch.Fk_FnID.ToString())).Value.TotalMinutes;
                    if (timeSpan <= 10)
                    { errMsg = "赛事开奖前10分钟内停止投注"; return false; }
                    else
                    {
                        Dictionary<int, List<SESENT_BasketBallGame>> keyValuePairs = new Dictionary<int, List<SESENT_BasketBallGame>>();
                        keyValuePairs.Add(type, Tools.RedisHelper.GetManger().GetWithList<SESENT_BasketBallGame>().Where(Z => Z.BasketballID == BaskBallID && Z.Type == type).ToList());
                        result.BallGames = keyValuePairs;
                        result.Match = basketBallMatch;
                        return true;
                    }

                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("CP查询支持接口:", err);
                return false;
            }
        }

        public bool MakeOrderWithFootBallGame(string AccountID,int lotteryId, string Fids, int type, int Multiple, out object result, out string errMsg)
        {
            result = null;
            errMsg = "投注失败!";
            try
            {
                Dictionary<string,string> ParseFids=UntilsObjToDic.ProductDetailList(Fids);
                if (ParseFids.Count < type) { errMsg = "选择的游戏场次大于玩法类型"; return false; }
                if (type == 1 && ParseFids.Count != 1) { errMsg = "单关类型和数据不匹配!";return false;}
                using (ModelContainer container = new ModelContainer())
                {
                    SESENT_USERS _USERS=container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (_USERS == null) { errMsg = "下注账号不存在!";return false; }
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    var item = ParseFids.GetEnumerator();
                    int count = ParseFids.Count;
                    List<string[]> list = new List<string[]>();
                    PublicDefined.GameType Gametype = (PublicDefined.GameType)type;
                    StringBuilder builder = new StringBuilder();
                    while (item.MoveNext())
                    {
                        long FootballID = long.Parse(item.Current.Key);
                        SESENT_FootBallGame sESENT = null;
                        SESENT_FootBallMatch match = null;
                        builder.Append(item.Current.Key).Append("|").Append(item.Current.Key).Append("&");
                        string[] splitFid = item.Current.Value.Split(',');
                        //过滤重复的
                        if (Verification.IsRepeatHashSet(splitFid)) { errMsg = "不可重复选择同一项";return false;}
                        if (count == 1)
                        {
                            sESENT = container.SESENT_FootBallGame.Where(a =>(a.FootballID== FootballID &&(a.Type == (int)PublicDefined.ZqGameType.LetBallWithSigler || a.Type == (int)PublicDefined.ZqGameType.NotLatBallWithSigler))).FirstOrDefault();
                            if (sESENT == null) { errMsg = "该场游戏未支持单关"; return false; }
                            match = container.SESENT_FootBallMatch.Where(a => a.FootballID == sESENT.FootballID).FirstOrDefault();
                            if (match == null) { errMsg = "未查询到该场比赛!"; return false; }
                        }
                        if (match == null)
                            match = container.SESENT_FootBallMatch.Where(a => a.FootballID == FootballID).FirstOrDefault();
                        if (DateTime.Now > match.MatchDate) { errMsg = "该场比赛投注时间已截至。"; return false; }
                        list.Add(splitFid);
                    }
                    builder.Remove(builder.Length - 1, 1);
                    XLetteryAlgorithm xLettery = new XLetteryAlgorithm(list);
                    List<string> WorkOutCount=xLettery.GetModelsWithType(Gametype).ToList();
                    long amount = WorkOutCount.Count * 2 * Multiple;
                    if (_USERS.UseAmount < amount) { errMsg = "账户余额不足!";return false;}
                    SESENT_FootBallOrder order = new SESENT_FootBallOrder()
                    {
                        EnterTime = DateTime.Now,
                        FIds = builder.ToString(),
                        OrderID = long.Parse(RuleUtility.RuleGenerateOrder.GetOrderID()),
                        Status = (int)PublicDefined.OrderStatus.wait,
                        Type = type
                    };
                    _USERS.UseAmount -= amount;
                    container.Entry(order).State = System.Data.Entity.EntityState.Added;
                    container.Entry(_USERS).State = System.Data.Entity.EntityState.Modified;
                    bool isSuccess=container.SaveChanges() > 0;
                    if (isSuccess)
                        result = "投注成功!尽请期待,祝君中奖。";
                    return isSuccess;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("CP查询支持接口:", err);
                return false;
            }
        }
    }
}
