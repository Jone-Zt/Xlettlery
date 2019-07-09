using Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RuleUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace XlettleryScrapy.ZqHandler
{
    public delegate void ZqListHandler(string path);
    public class ZqListScrapyHandler
    {
        private ZqListScrapyHandler(string path)
        {
            ZqListHandler = new ZqListHandler(Init);
            ZqListHandler.Invoke(path);
            driver.Close();
        }
        private ZqListHandler ZqListHandler;
        private ChromeDriver driver;
        private static ZqListScrapyHandler handler = null;
        private static object _sync = new object();
        public static ZqListScrapyHandler GetHandler(string path)
        {
            if (handler == null)
            {
                lock (_sync)
                {
                    Interlocked.CompareExchange(ref handler, new ZqListScrapyHandler(path), null);
                }
            }
            return handler;
        }
        public void Init(string path)
        {
            try
            {
                driver = new ChromeDriver();
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(6000);
                driver.Url = path;
                ParseHtml();
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError("爬虫抓取错误:"+err.Message);
            }
        }
        public void ParseHtml()
        {
            ReadOnlyCollection<IWebElement> btnWarps = driver.FindElementsByClassName("bet-date-wrap");
            var items = btnWarps.GetEnumerator();
            while (items.MoveNext())
            {
                var a = items.Current.FindElement(By.TagName("a"));
                var txt = a.FindElements(By.TagName("span"));
                if (txt[1].Text == "展开")
                    a.Click();
                var date = items.Current.FindElement(By.ClassName("bet-date")).Text; //赛事时间
                string[] dates = date.Split(' '); 
                var table = items.Current.FindElement(By.XPath("following-sibling::table[1]"));
                var atr = table.FindElement(By.TagName("tbody")).FindElements(By.ClassName("bet-tb-tr"));
                Model.ModelContainer container = new Model.ModelContainer();
                foreach (var item in atr)
                {
                    long FootBallID = RuleGenerateGame.GetGameID();
                    string No = string.Empty;//编号
                    string Evt = string.Empty;//赛事
                    string Endtime = string.Empty;//开赛时间
                    string Mainteam = string.Empty;//主队
                    string MainteamRanking = string.Empty;//主队排名
                    string Visitingteam = string.Empty;//客队
                    string VisitingteamRanking = string.Empty;//客队排名
                    string rangA1 = string.Empty;//不让球
                    string rangA2 = string.Empty;//让球
                    string F_Uid = string.Empty;//更新编号
                    DateTime time = DateTime.MaxValue;
                    foreach (var citem in item.FindElements(By.TagName("td")))
                    {
                        if (string.IsNullOrEmpty(citem.Text))
                            continue;
                        if (IsExitsWithClass(citem, "td td-no"))
                        {
                            var itmesx = citem.FindElement(By.TagName("a"));
                            No = itmesx.Text;
                            F_Uid = itmesx.GetAttribute("id");
                        }
                        else if (IsExitsWithClass(citem, "td td-evt"))
                            Evt = citem.FindElement(By.TagName("a")).Text;
                        else if (IsExitsWithClass(citem, "td td-endtime"))
                        {
                            Endtime = citem.Text;
                            string timeOut = DateTime.Now.ToString("yyyy") + "-" + Endtime;
                            time = DateTime.Parse(timeOut);
                        }
                        else if (IsExitsWithClass(citem, "td td-team"))
                        {
                            var cdiv = citem.FindElement(By.ClassName("team"));
                            var teamL = cdiv.FindElement(By.ClassName("team-l"));
                            MainteamRanking = teamL.FindElement(By.TagName("i")).Text;
                            Mainteam = teamL.FindElement(By.TagName("a")).Text;
                            var teamR = cdiv.FindElement(By.ClassName("team-r"));
                            Visitingteam = teamR.FindElement(By.TagName("a")).Text;
                            VisitingteamRanking = teamR.FindElement(By.TagName("i")).Text;
                        }
                        else if (IsExitsWithClass(citem, "td td-rang"))
                        {
                            var p = citem.FindElements(By.TagName("p"));
                            rangA1 = p[0].Text;
                            rangA2 = p[1].Text;
                        }
                        else if (IsExitsWithClass(citem, "td td-betbtn"))
                        {
                            var betDiv = citem.FindElements(By.TagName("div"));
                            //不让球比分获取
                            var j = 0;
                            foreach (var Plitem in betDiv[0].FindElements(By.TagName("p")))
                            {
                                string name=string.Empty;
                                if (j == 0)
                                    name = "胜";
                                else if (j == 1)
                                    name = "平";
                                else if (j == 2)
                                    name = "负";
                                Model.SESENT_FootBallGame game = new Model.SESENT_FootBallGame()
                                {
                                    FootballID = FootBallID,
                                    Name = name,
                                    Source = Plitem.Text,
                                    Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "1", j++),
                                    Type = rangA1.Contains("单关") ? (int)PublicDefined.ZqGameType.NotLatBallWithSigler : (int)PublicDefined.ZqGameType.NotLatball,
                                    Lable = rangA1,
                                };
                                SESENT_FootBallGame oldGame = container.SESENT_FootBallGame.Where(k => k.Code == game.Code).FirstOrDefault();
                                if (oldGame != null)
                                {
                                    oldGame.Name = game.Name;
                                    oldGame.Source = game.Source;
                                    container.Entry(oldGame).State = System.Data.Entity.EntityState.Modified;
                                    container.SaveChanges();
                                    Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_FootBallGame>(game.Code.ToString()), oldGame, time - DateTime.Now);
                                }
                                else
                                {
                                    container.SESENT_FootBallGame.Add(game);
                                    container.Entry(game).State = System.Data.Entity.EntityState.Added;
                                    container.SaveChanges();
                                    Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_FootBallGame>(game.Code.ToString()), game, time - DateTime.Now);
                                }
                            }
                            //让球比分
                            j = 0;
                            foreach (var Pritem in betDiv[1].FindElements(By.TagName("p")))
                            {
                                string name = string.Empty;
                                if (j == 0)
                                    name = "胜";
                                else if (j == 1)
                                    name = "平";
                                else if (j == 2)
                                    name = "负";
                                Model.SESENT_FootBallGame game = new Model.SESENT_FootBallGame()
                                {
                                    FootballID = FootBallID,
                                    Name = name,
                                    Source = Pritem.Text,
                                    Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "2",j++),
                                    Type =rangA2.Contains("单关")?(int)PublicDefined.ZqGameType.LetBallWithSigler:(int)PublicDefined.ZqGameType.Letball,
                                    Lable=rangA2
                                };
                                SESENT_FootBallGame oldGame = container.SESENT_FootBallGame.Where(k => k.Code == game.Code).FirstOrDefault();
                                if (oldGame != null)
                                {
                                    oldGame.Name = game.Name;
                                    oldGame.Source = game.Source;
                                    container.Entry(oldGame).State = System.Data.Entity.EntityState.Modified;
                                    container.SaveChanges();
                                    Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_FootBallGame>(game.Code.ToString()), oldGame, time - DateTime.Now);
                                }
                                else
                                {
                                    container.SESENT_FootBallGame.Add(game);
                                    container.Entry(game).State = System.Data.Entity.EntityState.Added;
                                    container.SaveChanges();
                                    Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_FootBallGame>(game.Code.ToString()), game, time - DateTime.Now);
                                }
                            }
                        }
                        else if (IsExitsWithClass(citem, "td td-more"))
                        {
                            var btn_more = citem.FindElement(By.ClassName("bet-more-btn")).FindElement(By.ClassName("bet-more-btn-txt"));
                            if (btn_more.Text == "展开")
                                btn_more.Click();
                            var moreItemtr = item.FindElement(By.XPath("following-sibling::tr[1]"));
                            foreach (var td_moreitem in moreItemtr.FindElement(By.TagName("td")).FindElements(By.TagName("table")))
                            {
                                var more_tr = td_moreitem.FindElement(By.TagName("tr"));
                                PublicDefined.ZqGameType type = PublicDefined.ZqGameType.DoubleResult;
                                string title = more_tr.FindElement(By.TagName("th")).Text;
                                string innerType = string.Empty;
                                switch (title)
                                {
                                    case "半全场":
                                        type = PublicDefined.ZqGameType.DoubleResult;
                                        innerType = "3";
                                        break;
                                    case "比分":
                                        type = PublicDefined.ZqGameType.Score;
                                        innerType = "4";
                                        break;
                                    case "进球数":
                                        type = PublicDefined.ZqGameType.NumberofGoalsScored;
                                        innerType = "5";
                                        break;
                                    default:
                                        throw new Exception("未知类型");
                                }
                                var j = 0;
                                foreach (var tditem in more_tr.FindElements(By.TagName("td")))
                                {
                                    string name = tditem.FindElement(By.TagName("p")).GetAttribute("data-value").TrimEnd();
                                    string source = tditem.FindElement(By.TagName("p")).FindElement(By.TagName("i")).Text;
                                    Model.SESENT_FootBallGame game = new Model.SESENT_FootBallGame()
                                    {
                                        FootballID = FootBallID,
                                        Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], innerType,j++),
                                        Name = name,
                                        Source = source,
                                        Type = (int)type,
                                    };
                                    SESENT_FootBallGame oldGame = container.SESENT_FootBallGame.Where(k => k.Code == game.Code).FirstOrDefault();
                                    if (oldGame != null)
                                    {
                                        oldGame.Name = game.Name;
                                        oldGame.Source = game.Source;
                                        container.Entry(oldGame).State = System.Data.Entity.EntityState.Modified;
                                        container.SaveChanges();
                                        Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_FootBallGame>(game.Code.ToString()), oldGame, time - DateTime.Now);
                                    }
                                    else
                                    {
                                        container.SESENT_FootBallGame.Add(game);
                                        container.Entry(game).State = System.Data.Entity.EntityState.Added;
                                        container.SaveChanges();
                                        Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_FootBallGame>(game.Code.ToString()), game, time - DateTime.Now);
                                    }
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(No))
                    {
                        SaveChange(new Model.SESENT_FootBallMatch()
                        {
                            Fk_FnID = RuleGenerateGame.Update_CacheID(F_Uid),
                            FootballID = FootBallID,
                            MatchDate = time,
                            MatchWeek = dates[1],
                            EndTime = Endtime,
                            Mainteam = Mainteam,
                            MainteamRanking = MainteamRanking,
                            No = No,
                            Visitingteam = Visitingteam,
                            VisitingteamRanking = VisitingteamRanking,
                            Match = Evt,
                        },time-DateTime.Now);
                    }
                }
            }
        }
        public bool SaveChange(Model.SESENT_FootBallMatch match,TimeSpan timeOut)
        {
            Model.ModelContainer container = new Model.ModelContainer();
            container.SESENT_FootBallMatch.Add(match);
            SESENT_FootBallMatch oldGame = container.SESENT_FootBallMatch.Where(k => k.Fk_FnID == match.Fk_FnID).FirstOrDefault();
            if (oldGame == null)
            {
                container.SESENT_FootBallMatch.Add(match);
                container.Entry(match).State = System.Data.Entity.EntityState.Added;
                bool isSuccess = container.SaveChanges() > 0;
                if (isSuccess)
                    Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_FootBallMatch>(match.Fk_FnID.ToString()), match, timeOut);
                return isSuccess;
            }
            return true;
        }
        public bool IsExitsWithClass(IWebElement element,string className)
        {
            try
            {
                string result = element.GetAttribute("class");
                if (className == result)
                    return true;
                else
                    return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
