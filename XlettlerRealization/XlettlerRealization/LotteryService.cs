﻿using AOPHandlerManager.MethordHandler;
using DealManagement;
using Model;
using Newtonsoft.Json;
using PublicDefined;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Tools;

namespace XlettlerRealization
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class LotteryService : IlotteryInterface
    {
        public bool QueryBasketBallLottery(int lotteryId, out Dictionary<string,List<MySlefGeneratePicker<dynamic, Dictionary<string, object>>>> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = new Dictionary<string,List<MySlefGeneratePicker<dynamic, Dictionary<string, object>>>>();
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open && a.Type == (int)PublicDefined.LetteryType.BasketBall).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    DateTime dataNow = DateTime.Now;
                    for (int i = 0; i < 3; i++)
                    {
                        dataNow = DateTime.Parse(dataNow.ToString("yyyy-MM-dd 0:0:0"));
                        List<Model.SESENT_BasketBallMatch> list = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_BasketBallMatch>();
                        List<MySlefGeneratePicker<object, Dictionary<string, object>>> plist = new List<MySlefGeneratePicker<object, Dictionary<string, object>>>();
                        IEnumerable<SESENT_BasketBallMatch> matches = list.Where(a => a.MatchDate == dataNow);
                        var item = matches.GetEnumerator();
                        while (item.MoveNext())
                        {
                            MySlefGeneratePicker<dynamic, Dictionary<string, object>> picker = new MySlefGeneratePicker<dynamic, Dictionary<string, object>>();
                            Dictionary<string, object> Match = UntilsObjToDic.ToMap(item.Current);
                            if (Match != null)
                            {
                                Match["BasketballID"] = item.Current.BasketballID.ToString();
                                Match["MatchDate"] = item.Current.MatchDate.ToString("yyyy-MM-dd");
                                Match.Remove("Fk_FnID");
                            }
                            picker.Match = Match;
                            List<Model.SESENT_BasketBallGame> games = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_BasketBallGame>();
                            dynamic letBall = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.Letball).Select(a => new
                            {
                                BasketballID = a.BasketballID.ToString(),
                                a.Fid,
                                a.Lable,
                                a.Name,
                                a.Type,
                                a.Source,
                            });
                            picker.BallGames.Add((int)PublicDefined.LqGageType.Letball, letBall);
                            dynamic doubleResult = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.SizeSoure).Select(a => new
                            {
                                BasketballID = a.BasketballID.ToString(),
                                a.Fid,
                                a.Lable,
                                a.Name,
                                a.Type,
                                a.Source,
                            });
                            picker.BallGames.Add((int)PublicDefined.LqGageType.SizeSoure, doubleResult);
                            dynamic NumberofGoalsScored = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.VictoryOrFail).Select(a => new
                            {
                                BasketballID = a.BasketballID.ToString(),
                                a.Fid,
                                a.Lable,
                                a.Name,
                                a.Type,
                                a.Source,
                            });
                            picker.BallGames.Add((int)PublicDefined.LqGageType.VictoryOrFail, NumberofGoalsScored);
                            dynamic Score = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.VictoryOrFailDiff_Main).Select(a => new
                            {
                                BasketballID = a.BasketballID.ToString(),
                                a.Fid,
                                a.Lable,
                                a.Name,
                                a.Type,
                                a.Source,
                            });
                            picker.BallGames.Add((int)PublicDefined.LqGageType.VictoryOrFailDiff_Main, Score);
                            dynamic VictoryOrFailDiff_Visable = games.Where(a => a.BasketballID == item.Current.BasketballID && a.Type == (int)PublicDefined.LqGageType.VictoryOrFailDiff_Visable).Select(a => new
                            {
                                BasketballID = a.BasketballID.ToString(),
                                a.Fid,
                                a.Lable,
                                a.Name,
                                a.Type,
                                a.Source,
                            });
                            picker.BallGames.Add((int)PublicDefined.LqGageType.VictoryOrFailDiff_Visable, VictoryOrFailDiff_Visable);
                            plist.Add(picker);
                        }
                        result.Add(dataNow.ToString("yyyy-MM-dd"), plist);
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
        public bool QueryFootBallLotteryWithType(int lotteryId, long FootBallID, int? type, out MySlefGeneratePicker<dynamic, Dictionary<string, object>> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = new MySlefGeneratePicker<dynamic, Dictionary<string, object>>();
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open && a.Type == (int)PublicDefined.LetteryType.FootBall).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    SESENT_FootBallMatch footBallMatch = container.SESENT_FootBallMatch.Where(a => a.FootballID == FootBallID).FirstOrDefault();
                    if (footBallMatch == null) { errMsg = "未查询到该场次!"; return false; }
                    double timeSpan = Tools.RedisHelper.GetManger().GetTimeOut(Tools.CacheKey.GenerateCacheGameBall<SESENT_FootBallMatch>(footBallMatch.Fk_FnID.ToString())).Value.TotalMinutes;
                    if (timeSpan <= 10)
                    { errMsg = "赛事开奖前10分钟内停止投注"; return false; }
                    else
                    {
                        IEnumerable<SESENT_FootBallGame> ballGames = Tools.RedisHelper.GetManger().GetWithList<SESENT_FootBallGame>().Where(Z => Z.FootballID == FootBallID);
                        Dictionary<string, object> Match = UntilsObjToDic.ToMap(footBallMatch);
                        if (Match != null)
                        {
                            Match["FootballID"] = footBallMatch.FootballID.ToString();
                            Match["MatchDate"] = footBallMatch.MatchDate.ToString("yyyy-MM-dd");
                            Match.Remove("Fk_FnID");
                        }
                        if (type != null)
                        {
                            PublicDefined.ZqGameType lqGametype = (PublicDefined.ZqGameType)type;
                            var resultType = ballGames.Where(a => a.Type == type).Select(b => new
                            {
                                FootballID = b.FootballID.ToString(),
                                b.FId,
                                b.Lable,
                                b.Name,
                                b.Source,
                                b.Type
                            }).ToList();
                            Dictionary<int, DataTable> dic = new Dictionary<int, DataTable>();
                            dic.Add((int)lqGametype, UntilsObjToDic.ListToDataTable(resultType));
                            result.BallGames = dic;
                            result.Match = Match;
                        }
                        else
                        {
                            Dictionary<int, DataTable> keyValuePairs = new Dictionary<int, DataTable>();
                            dynamic Letball = ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.Letball).Select(b =>
       new
       {
           FootballID = b.FootballID.ToString(),
           b.FId,
           b.Lable,
           b.Name,
           b.Source,
           b.Type
       }).ToList();
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.Letball, UntilsObjToDic.ListToDataTable(Letball));

                            dynamic DoubleResult = ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.DoubleResult).Select(b =>
           new
           {
               FootballID = b.FootballID.ToString(),
               b.FId,
               b.Lable,
               b.Name,
               b.Source,
               b.Type
           }).ToList();
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.DoubleResult, UntilsObjToDic.ListToDataTable(DoubleResult));
                            dynamic LetBallWithSigler = ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.LetBallWithSigler).Select(b =>
          new
          {
              FootballID = b.FootballID.ToString(),
              b.FId,
              b.Lable,
              b.Name,
              b.Source,
              b.Type
          }).ToList();
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.LetBallWithSigler, UntilsObjToDic.ListToDataTable(LetBallWithSigler));
                            dynamic NotLatball = ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.NotLatball).Select(b =>
           new
           {
               FootballID = b.FootballID.ToString(),
               b.FId,
               b.Lable,
               b.Name,
               b.Source,
               b.Type
           }).ToList();
                            keyValuePairs.Add((int)PublicDefined.ZqGameType.NotLatball, UntilsObjToDic.ListToDataTable(NotLatball));
                            dynamic NotLatBallWithSigler = ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.NotLatBallWithSigler).Select(b =>
         new
         {
             FootballID = b.FootballID.ToString(),
             b.FId,
             b.Lable,
             b.Name,
             b.Source,
             b.Type
         }).ToList();

                            keyValuePairs.Add((int)PublicDefined.ZqGameType.NotLatBallWithSigler, UntilsObjToDic.ListToDataTable(NotLatBallWithSigler));
                            dynamic NumberofGoalsScored = ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.NumberofGoalsScored).Select(b =>
         new
         {
             FootballID = b.FootballID.ToString(),
             b.FId,
             b.Lable,
             b.Name,
             b.Source,
             b.Type
         }).ToList();

                            keyValuePairs.Add((int)PublicDefined.ZqGameType.NumberofGoalsScored, UntilsObjToDic.ListToDataTable(NumberofGoalsScored));
                            dynamic Score = ballGames.Where(Z => Z.Type == (int)PublicDefined.ZqGameType.Score).Select(b =>
           new
           {
               FootballID = b.FootballID.ToString(),
               b.FId,
               b.Lable,
               b.Name,
               b.Source,
               b.Type
           }).ToList();
                            result.BallGames = keyValuePairs;
                            result.Match = Match;
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
        public bool QueryFootBallLotteryWithMeach(int lotteryId, int type, out Dictionary<string,List<MySlefGeneratePicker<object, Dictionary<string, object>>>> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    PublicDefined.ZqGameType zqtype = (PublicDefined.ZqGameType)type;
                    result = new Dictionary<string,List<MySlefGeneratePicker<object, Dictionary<string, object>>>>();
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open && a.Type == (int)PublicDefined.LetteryType.FootBall).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    DateTime dataNow = DateTime.Now;
                    List<Model.SESENT_FootBallMatch> list = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_FootBallMatch>().OrderBy(a=>a.No).ToList();
                    List<Model.SESENT_FootBallGame> games = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_FootBallGame>();
                    List<MySlefGeneratePicker<object, Dictionary<string, object>>> plist =null;
                    for (int i = 0; i < 3; i++)
                    {
                        plist=new List<MySlefGeneratePicker<object, Dictionary<string, object>>>();
                          IEnumerable <SESENT_FootBallMatch> matches = list.Where(a => a.MatchDate.ToString("yyyy-MM-dd") == dataNow.ToString("yyyy-MM-dd"));
                        var item = matches.GetEnumerator();
                        while (item.MoveNext())
                        {
                            MySlefGeneratePicker<object, Dictionary<string, object>> picker = new MySlefGeneratePicker<object, Dictionary<string, object>>();
                            Dictionary<string, object> Match = UntilsObjToDic.ToMap(item.Current);
                            if (Match != null)
                            {
                                Match["FootballID"] = item.Current.FootballID.ToString();
                                Match["MatchDate"] = item.Current.MatchDate.ToString("yyyy-MM-dd");
                                Match.Remove("Fk_FnID");
                            }
                            picker.Match = Match;
                            var zqtypeList = games.Where(a => a.FootballID == item.Current.FootballID && a.Type == (int)zqtype);
                            var zqtypeListSelect = zqtypeList.Select(b => new
                            {
                                b.FootballID,
                                b.FId,
                                b.Lable,
                                b.Name,
                                b.Source,
                                b.Type
                            }).ToList();
                            picker.BallGames.Add((int)zqtype, UntilsObjToDic.ListToDataTable(zqtypeListSelect));
                            plist.Add(picker);
                        }
                        result.Add(dataNow.ToString("yyyy-MM-dd"), plist);
                        dataNow = dataNow.AddDays(1);
                    }
                    return true;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("CP查询支持接口1:", err);
                return false;
            }
        }
        public bool QueryFootBallLottery(int lotteryId, out Dictionary<string,List<MySlefGeneratePicker<object, Dictionary<string, object>>>> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = new Dictionary<string,List<MySlefGeneratePicker<object, Dictionary<string, object>>>>();
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open && a.Type == (int)PublicDefined.LetteryType.FootBall).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    DateTime dataNow = DateTime.Now;
                    List<Model.SESENT_FootBallMatch> list = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_FootBallMatch>().OrderBy(a=>a.No).ToList();
                    List<Model.SESENT_FootBallGame> games = Tools.RedisHelper.GetManger().GetWithList<Model.SESENT_FootBallGame>();
                    List<MySlefGeneratePicker<object, Dictionary<string, object>>> plist = null;
                    for (int i = 0; i < 3; i++)
                    {
                        plist=new List<MySlefGeneratePicker<object, Dictionary<string, object>>>();
                        //查询当前的赛事
                        IEnumerable<SESENT_FootBallMatch> matches = list.Where(a => a.MatchDate.ToString("yyyy-MM-dd") == dataNow.ToString("yyyy-MM-dd"));
                        var item = matches.GetEnumerator();
                        while (item.MoveNext())
                        {
                            MySlefGeneratePicker<object, Dictionary<string, object>> picker = new MySlefGeneratePicker<object, Dictionary<string, object>>();
                            Dictionary<string, object> Match = UntilsObjToDic.ToMap(item.Current);
                            if (Match != null)
                            {
                                Match["FootballID"] = item.Current.FootballID.ToString();
                                Match["MatchDate"] = item.Current.MatchDate.ToString("yyyy-MM-dd");
                                Match.Remove("Fk_FnID");
                            }
                            picker.Match = Match;
                            //默认的两种玩法
                            var letBall = games.Where(a => a.FootballID == item.Current.FootballID && a.Type == (int)PublicDefined.ZqGameType.Letball);
                            var Letball = letBall.Select(b => new
                            {
                                b.FootballID,
                                b.FId,
                                b.Lable,
                                b.Name,
                                b.Source,
                                b.Type
                            }).ToList();
                            if (Letball == null && Letball.Count > 0)
                            {
                                letBall= games.Where(a => a.FootballID == item.Current.FootballID && a.Type == (int)PublicDefined.ZqGameType.LetBallWithSigler);
                                Letball = letBall.Select(b => new
                                {
                                    b.FootballID,
                                    b.FId,
                                    b.Lable,
                                    b.Name,
                                    b.Source,
                                    b.Type
                                }).ToList();
                                picker.BallGames.Add((int)PublicDefined.ZqGameType.NotLatBallWithSigler, UntilsObjToDic.ListToDataTable(Letball));
                            }
                            else
                              picker.BallGames.Add((int)PublicDefined.ZqGameType.Letball, UntilsObjToDic.ListToDataTable(Letball));
                            var NotletBall = games.Where(a => a.FootballID == item.Current.FootballID && a.Type == (int)PublicDefined.ZqGameType.NotLatball);
                            var notlaballGame = NotletBall.Select(b => new
                            {
                                b.FootballID,
                                b.FId,
                                b.Lable,
                                b.Name,
                                b.Source,
                                b.Type
                            }).ToList();
                            if (notlaballGame == null&&notlaballGame.Count>0)
                            {
                                NotletBall = games.Where(a => a.FootballID == item.Current.FootballID && a.Type == (int)PublicDefined.ZqGameType.NotLatBallWithSigler);
                                notlaballGame = NotletBall.Select(b => new
                                {
                                    b.FootballID,
                                    b.FId,
                                    b.Lable,
                                    b.Name,
                                    b.Source,
                                    b.Type
                                }).ToList();
                                picker.BallGames.Add((int)PublicDefined.ZqGameType.NotLatBallWithSigler, UntilsObjToDic.ListToDataTable(notlaballGame));
                            }
                            else
                               picker.BallGames.Add((int)PublicDefined.ZqGameType.NotLatball, UntilsObjToDic.ListToDataTable(notlaballGame));
                            plist.Add(picker);
                        }
                        result.Add(dataNow.ToString("yyyy-MM-dd"),plist);
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
        public bool QueryBasketBallLotteryWithType(int lotteryId, long BaskBallID, int type, out MySlefGeneratePicker<dynamic, Dictionary<string, object>> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    result = new MySlefGeneratePicker<dynamic, Dictionary<string, object>>();
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open && a.Type == (int)PublicDefined.LetteryType.BasketBall).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    SESENT_BasketBallMatch basketBallMatch = container.SESENT_BasketBallMatch.Where(a => a.BasketballID == BaskBallID).FirstOrDefault();
                    if (basketBallMatch == null) { errMsg = "未查询到该场次!"; return false; }
                    Dictionary<string, object> Match = UntilsObjToDic.ToMap(basketBallMatch);
                    if (Match != null)
                    {
                        Match["BasketballID"] = basketBallMatch.BasketballID.ToString();
                        Match["MatchDate"] = basketBallMatch.MatchDate.ToString("yyyy-MM-dd");
                        Match.Remove("Fk_FnID");
                    }
                    double timeSpan = Tools.RedisHelper.GetManger().GetTimeOut(Tools.CacheKey.GenerateCacheGameBall<SESENT_BasketBallMatch>(basketBallMatch.Fk_FnID.ToString())).Value.TotalMinutes;
                    if (timeSpan <= 10)
                    { errMsg = "赛事开奖前10分钟内停止投注"; return false; }
                    else
                    {
                        Dictionary<int, DataTable> keyValuePairs = new Dictionary<int, DataTable>();
                        dynamic res = Tools.RedisHelper.GetManger().GetWithList<SESENT_BasketBallGame>().Where(Z => Z.BasketballID == BaskBallID && Z.Type == type).Select(b => new
                        {
                            BasketballID = b.BasketballID.ToString(),
                            b.Fid,
                            b.Lable,
                            b.Name,
                            b.Source,
                            b.Type
                        });
                        keyValuePairs.Add(type, res);
                        result.BallGames = keyValuePairs;
                        result.Match = Match;
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
        public bool MakeOrderWithFootBallGame(string AccountID, int lotteryId, string Fids, int[] type, int Multiple, out object result, out string errMsg)
        {
            result = null;
            errMsg = "投注失败!";
            try
            {
                Dictionary<string, string> ParseFids = UntilsObjToDic.ProductDetailList(Fids);
                string makeOrders=JsonConvert.SerializeObject(ParseFids);
                if (type.Where(a => a > ParseFids.Count).Count() > 0) { errMsg = "选择的游戏场次大于玩法类型"; return false; }
                using (ModelContainer container = new ModelContainer())
                {
                    SESENT_USERS _USERS = container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (_USERS == null) { errMsg = "下注账号不存在!"; return false; }
                    SESENT_Lottery sESENT_Lottery = container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Status == (int)PublicDefined.Status.Open).FirstOrDefault();
                    if (sESENT_Lottery == null) { errMsg = "未开放该游戏,敬请期待。"; return false; }
                    var item = ParseFids.GetEnumerator();
                    int count = ParseFids.Count;
                    List<string[]> list = new List<string[]>();
                    while (item.MoveNext())
                    {
                        long FootballID = long.Parse(item.Current.Key);
                        SESENT_FootBallGame sESENT = null;
                        SESENT_FootBallMatch match = null;
                        string[] splitFid = item.Current.Value.Split(',');
                        //过滤重复的
                        if (Verification.IsRepeatHashSet(splitFid)) { errMsg = "不可重复选择同一项"; return false; }
                        if (count == 1)
                        {
                            sESENT = container.SESENT_FootBallGame.Where(a => (a.FootballID == FootballID && (a.Type == (int)PublicDefined.ZqGameType.LetBallWithSigler || a.Type == (int)PublicDefined.ZqGameType.NotLatBallWithSigler))).FirstOrDefault();
                            if (sESENT == null) { errMsg = "该场游戏未支持单关"; return false; }
                            match = container.SESENT_FootBallMatch.Where(a => a.FootballID == sESENT.FootballID).FirstOrDefault();
                            if (match == null) { errMsg = "未查询到该场比赛!"; return false; }
                        }
                        if (match == null)
                            match = container.SESENT_FootBallMatch.Where(a => a.FootballID == FootballID).FirstOrDefault();
                        if (DateTime.Now > match.MatchDate) { errMsg = "该场比赛投注时间已截至。"; return false; }
                        list.Add(splitFid);
                    }
                    long workcount = 0;
                    XLetteryAlgorithm xLettery = new XLetteryAlgorithm(list);
                    for (int i = 0; i < type.Length; i++)
                    {
                        PublicDefined.GameType Gametype = (PublicDefined.GameType)type[i];
                        workcount += xLettery.GetModelsWithType(Gametype).Count;
                    }
                    long amount = workcount * 2 * Multiple;
                    if (_USERS.UseAmount < amount) { errMsg = "账户余额不足!"; return false; }
                    string parseType = string.Join(",", type.Select(i => i.ToString()).ToArray());
                    SESENT_BallOrder order = new SESENT_BallOrder()
                    {
                        AccountID=AccountID,
                        GameType=(int)PublicDefined.LetteryType.FootBall,
                        EnterTime = DateTime.Now,
                        FIds = makeOrders,
                        OrderID = long.Parse(RuleUtility.RuleGenerateOrder.GetOrderID()),
                        Status = (int)PublicDefined.OrderStatus.wait,
                        Type = parseType,
                        Amount=amount
                    };
                    _USERS.UseAmount -= amount;
                    container.Entry(order).State = System.Data.Entity.EntityState.Added;
                    container.Entry(_USERS).State = System.Data.Entity.EntityState.Modified;
                    bool isSuccess = container.SaveChanges() > 0;
                    if (isSuccess)
                    {
                        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                        keyValuePairs.Add("OrderID", order.OrderID.ToString());
                        keyValuePairs.Add("PayAmount", amount.ToString("0.00"));
                        keyValuePairs.Add("GameType",((int)PublicDefined.LetteryType.FootBall).ToString());
                        keyValuePairs.Add("ChargeBalance", _USERS.UseAmount.ToString("0.00"));
                        result = keyValuePairs;
                    }
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
        public bool QueryOrderWithBall(string AccountID,bool Type,DateTime startTime,DateTime EndTime,out DataTable result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                PublicDefined.OrderStatus orderStatus = OrderStatus.wait;
                using (ModelContainer container = new ModelContainer())
                {
                  SESENT_USERS uSERS=container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (uSERS == null){errMsg = "未查询到该账号";return false;}
                    IQueryable<SESENT_BallOrder> list = null;
                    startTime = DateTime.Parse(startTime.ToString("yyyy-MM-dd 0:0:0"));
                    EndTime = DateTime.Parse(EndTime.ToString("yyyy-MM-dd 23:59:59"));
                    if (Type)
                        list = container.SESENT_BallOrder.Where(a => a.AccountID == AccountID && a.Status != (int)orderStatus&&(a.EnterTime>=startTime&&a.EnterTime<=EndTime));
                    else
                        list = container.SESENT_BallOrder.Where(a => a.AccountID == AccountID && a.Status == (int)orderStatus && (a.EnterTime >= startTime && a.EnterTime <= EndTime));
                 var itemTor=list.GetEnumerator();
                    List<object> kayOrders = new List<object>();
                    while (itemTor.MoveNext())
                    {
                        SESENT_BallOrder ballOrder = itemTor.Current;
                        DateTime time = ballOrder.EnterTime;
                        int Yeay = time.Year;
                        int Mouth = time.Month;
                        int Day = time.Day;
                        string Week = Verification.CaculateWeekDay(Yeay, Mouth, Day);
                        kayOrders.Add(new
                        {
                            Date = Mouth + "/" + Day,
                            OrderID=ballOrder.OrderID,
                            Week=Week,
                            GameType=ballOrder.GameType,
                            Status=ballOrder.Status,
                            Amount=ballOrder.Amount,
                        });
                    }
                    result = UntilsObjToDic.ListToDataTable(kayOrders);
                    return true;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError("CP查询足球投注订单接口错误:", err);
                return false;
            }
        }

        public bool MakeUserFollow(string AccountID, string FollowID,int type,out string result, out string errMsg)
        {
            result = string.Empty;
            errMsg = string.Empty;
            try
            {
                FollowType followType = (FollowType)type;
                using (Model.ModelContainer container = new ModelContainer())
                {
                    SESENT_USERS uSERS=container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (uSERS == null) { errMsg = "用户不存在!";return false; }
                    SENENT_GuanZhu sENENT=container.SENENT_GuanZhu.Where(a => a.AccountID == AccountID && a.followAccountID == FollowID).FirstOrDefault();
                    if (sENENT == null && followType == FollowType.UnFollow) { errMsg = "您未关注该用户";return false; }
                    if (sENENT == null) { sENENT.AccountID = AccountID;sENENT.followAccountID = FollowID;}
                    sENENT.Status = (short)followType;
                    container.SaveChanges();
                    result = "操作成功!";
                    return true;
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError($"操作用户关注接口失败:{err}");
                return false;
            }
        }

        public bool QueryUserFollow(string AccountID, out DataTable result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (Model.ModelContainer container = new ModelContainer()) 
                {
                    SESENT_USERS sESENT=container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (sESENT == null) { errMsg = "未查询到该账户！";return false; }
                    IQueryable<SENENT_GuanZhu> guanZhus=container.SENENT_GuanZhu.Where(a => a.AccountID == AccountID&&a.Status==(short)PublicDefined.FollowType.Follow);
                    var itemtor = guanZhus.GetEnumerator();
                    List<object> list = new List<object>();
                    while (itemtor.MoveNext())
                    {
                        //TODO 查询当前关注的是否正在有发单的订单(未开始)测试默认写的3
                        string nikeName = string.Empty;
                        string HeadImg = string.Empty;
                        SESENT_USERS uSERS=container.SESENT_USERS.Where(a => a.AccountID == itemtor.Current.followAccountID).FirstOrDefault();
                        if (uSERS != null) { nikeName = uSERS.userName;HeadImg = uSERS.HeadImg == null ? null : Convert.ToBase64String(uSERS.HeadImg); }
                        list.Add(new { nikeName,HeadImg, GenDanIngCount=3 });
                    }
                    result = UntilsObjToDic.ListToDataTable(list.ToList());
                    return true;
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError($"查询用户关注接口错误:【账号:{AccountID}】错误:{err}");
                errMsg = "未知错误!";
                return false;
            }
        }

        public bool MakeGenDan(string AccountID, int lotteryId, int GameType, string Fids, int[] Games, int Multiple, out object result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                Dictionary<string, string> ParseFids = UntilsObjToDic.ProductDetailList(Fids);
                if (Games.Where(a => a > ParseFids.Count).Count() > 0) { errMsg = "选择的游戏场次大于玩法类型"; return false; }
                using (Model.ModelContainer container = new ModelContainer()) 
                {
                    GameType type = (GameType)GameType;
                    SESENT_Lottery lottery=container.SESENT_Lottery.Where(a => a.lotteryId == lotteryId && a.Type == GameType&&a.Status==(int)PublicDefined.Status.Open).FirstOrDefault();
                    if (lottery == null) { errMsg = "当前游戏暂未开放!";return false; }
                    SESENT_USERS uSERS=container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (uSERS == null) { errMsg = "当前账户不存在!";return false; }
                    return true;
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError($"发起跟单接口失败:【账号:{AccountID}】{err.Message}");
                return false;
            }
        }

        public bool QueryOrderWithBallDetail(string AccountID,int OrderID, out DataTable result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (Model.ModelContainer container = new ModelContainer())
                {
                    SESENT_USERS uSERS=container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (uSERS == null) { errMsg = "未查询到该用户!";return false; }
                    SESENT_BallOrder ballOrder=container.SESENT_BallOrder.Where(a=>a.AccountID==uSERS.AccountID&&a.OrderID==OrderID).FirstOrDefault();
                    if (ballOrder == null) { errMsg = "未查询到该订单!";return false; }
                    if (ballOrder.GameType == (int)LetteryType.FootBall)
                    {
                      Dictionary<string,string> keyValuePairs=UntilsObjToDic.ProductDetailList(ballOrder.FIds);
                      var itemtor = keyValuePairs.GetEnumerator();
                        while (itemtor.MoveNext())
                        {

                        }
                        return true;
                    }
                    else
                    {
                        errMsg = "未开放其他玩法查询接口!";
                        return false;
                    }
                }
            }
            catch (Exception err)
            {
                errMsg = "未知错误!";
                LogTool.LogWriter.WriteError($"查询投注详情:【订单号:{OrderID}】{err}");
                return false;
            }
        }
    }
}
