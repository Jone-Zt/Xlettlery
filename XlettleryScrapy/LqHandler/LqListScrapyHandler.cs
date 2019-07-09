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
using System.Threading.Tasks;

namespace XlettleryScrapy.LqHandler
{
    public delegate void LqListHandler(string path);
    public class LqListScrapyHandler
    {
        private LqListScrapyHandler(string path)
        {
            ZqListHandler = new LqListHandler(Init);
            ZqListHandler.Invoke(path);
            driver.Close();
        }
        private LqListHandler ZqListHandler;
        private ChromeDriver driver;
        private static LqListScrapyHandler handler = null;
        private static object _sync = new object();
        public static LqListScrapyHandler GetHandler(string path)
        {
            if (handler == null)
            {
                lock (_sync)
                {
                    Interlocked.CompareExchange(ref handler, new LqListScrapyHandler(path), null);
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
                LogTool.LogWriter.WriteError("爬虫抓取错误:" + err.Message);
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
                foreach (var item in atr)
                {
                    long Basketball = RuleGenerateGame.GetGameID();
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
                    Model.ModelContainer container = new Model.ModelContainer();
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
                        else if (IsExitsWithClass(citem, "td td-betbtn td-bdr"))
                        {
                            PublicDefined.LqGageType type = PublicDefined.LqGageType.Letball;
                            var classNameItem = citem.FindElement(By.TagName("div"));
                            string className = classNameItem.GetAttribute("class");
                            switch (className.ToLower())
                            {
                                case "betbtn-row betbtn-row-sf":
                                    {
                                        int l = 0;
                                        type = PublicDefined.LqGageType.VictoryOrFail;
                                        var pItmesSf = classNameItem.FindElements(By.TagName("p"));
                                        var fail = pItmesSf[0].FindElement(By.TagName("span")).Text; //失败1
                                        var ver = pItmesSf[1].FindElement(By.TagName("span")).Text;//胜利2
                                        SESENT_BasketBallGame game = new SESENT_BasketBallGame()
                                        {
                                            BasketballID = Basketball,
                                            Name = "1",
                                            Source = fail,
                                            Type = (int)type,
                                            Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "1", l++),
                                        };
                                        SESENT_BasketBallGame oldGame = container.SESENT_BasketBallGame.Where(k => k.Code == game.Code).FirstOrDefault();
                                        SaveOrUpdate(oldGame, game, container, time);
                                        SESENT_BasketBallGame gamever = new SESENT_BasketBallGame()
                                        {
                                            BasketballID = Basketball,
                                            Name = "2",
                                            Source = ver,
                                            Type = (int)type,
                                            Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "2", l++),
                                        };
                                        SESENT_BasketBallGame oldverGame = container.SESENT_BasketBallGame.Where(k => k.Code == gamever.Code).FirstOrDefault();
                                        SaveOrUpdate(oldverGame, gamever, container, time);
                                        break;
                                    }
                                case "betbtn-row betbtn-row-rfsf":
                                    {
                                        int p = 0;
                                        type = PublicDefined.LqGageType.Letball;
                                        var pitems = classNameItem.FindElements(By.TagName("p"));
                                        var fail = pitems[0].FindElement(By.TagName("span")).Text;//1
                                        var lab = pitems[1].FindElement(By.TagName("span")).Text;//0
                                        var ver = pitems[2].FindElement(By.TagName("span")).Text;//2
                                        SESENT_BasketBallGame game = new SESENT_BasketBallGame()
                                        {
                                            BasketballID = Basketball,
                                            Name = "1",
                                            Source = fail,
                                            Type = (int)type,
                                            Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "3", p++),
                                        };
                                        SESENT_BasketBallGame oldGame = container.SESENT_BasketBallGame.Where(k => k.Code == game.Code).FirstOrDefault();
                                        SaveOrUpdate(oldGame, game, container, time);

                                        SESENT_BasketBallGame game1 = new SESENT_BasketBallGame()
                                        {
                                            BasketballID = Basketball,
                                            Name = "2",
                                            Source = ver,
                                            Type = (int)type,
                                            Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "4", p++),
                                        };
                                        SESENT_BasketBallGame oldGame1 = container.SESENT_BasketBallGame.Where(k => k.Code == game1.Code).FirstOrDefault();
                                        SaveOrUpdate(oldGame1, game1, container, time);

                                        SESENT_BasketBallGame game2 = new SESENT_BasketBallGame()
                                        {
                                            BasketballID = Basketball,
                                            Lable = "0",
                                            Source = ver,
                                            Type = (int)type,
                                            Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "5", p++),
                                        };
                                        SESENT_BasketBallGame oldGame2 = container.SESENT_BasketBallGame.Where(k => k.Code == game2.Code).FirstOrDefault();
                                        SaveOrUpdate(oldGame2, game2, container, time);
                                        break;
                                    }
                                case "betbtn-row betbtn-row-dxf":
                                    int j = 0;
                                    type = PublicDefined.LqGageType.SizeSoure;
                                    var Pitems = classNameItem.FindElements(By.TagName("p"));
                                    var Big = Pitems[0].FindElement(By.TagName("span")).Text;
                                    var lable = Pitems[1].FindElement(By.TagName("span")).Text;
                                    var small = Pitems[2].FindElement(By.TagName("span")).Text;
                                    SESENT_BasketBallGame game3 = new SESENT_BasketBallGame()
                                    {
                                        BasketballID = Basketball,
                                        Name = "1",
                                        Source = small,
                                        Type = (int)type,
                                        Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "6", j++),
                                    };
                                    SESENT_BasketBallGame oldGame3 = container.SESENT_BasketBallGame.Where(k => k.Code == game3.Code).FirstOrDefault();
                                    SaveOrUpdate(oldGame3, game3, container, time);

                                    SESENT_BasketBallGame game4 = new SESENT_BasketBallGame()
                                    {
                                        BasketballID = Basketball,
                                        Name = "2",
                                        Source = Big,
                                        Type = (int)type,
                                        Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "7", j++),
                                    };
                                    SESENT_BasketBallGame oldGame4 = container.SESENT_BasketBallGame.Where(k => k.Code == game4.Code).FirstOrDefault();
                                    SaveOrUpdate(oldGame4, game4, container, time);

                                    SESENT_BasketBallGame game5 = new SESENT_BasketBallGame()
                                    {
                                        BasketballID = Basketball,
                                        Lable = "0",
                                        Source = lable,
                                        Type = (int)type,
                                        Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "8", j++),
                                    };
                                    SESENT_BasketBallGame oldGame5 = container.SESENT_BasketBallGame.Where(k => k.Code == game5.Code).FirstOrDefault();
                                    SaveOrUpdate(oldGame5, game5, container, time);
                                    break;
                                default:
                                    throw new Exception("未查询到该类型!");
                            }
                        }
                        else if (IsExitsWithClass(citem, "td td-more"))
                        {
                            var spansItems = citem.FindElements(By.TagName("span"));
                            if (spansItems[0].Text == "展开")
                                spansItems[0].Click();
                            var moreItemtr = item.FindElement(By.XPath("following-sibling::tr[1]")).FindElement(By.TagName("td")).FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
                            int ld = 0;
                            foreach (var sitem in moreItemtr[0].FindElements(By.TagName("td")))
                            {
                                var iir = sitem.FindElement(By.TagName("p"));
                                string name =iir.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0];
                                string source = iir.GetAttribute("data-sp");
                                SESENT_BasketBallGame ballGame = new SESENT_BasketBallGame()
                                {
                                    BasketballID = Basketball,
                                    Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "9", ld++),
                                    Name = name,
                                    Source = source,
                                    Type =(int)PublicDefined.LqGageType.VictoryOrFailDiff_Visable,
                                };
                                SESENT_BasketBallGame oldGame6 = container.SESENT_BasketBallGame.Where(k => k.Code == ballGame.Code).FirstOrDefault();
                                SaveOrUpdate(oldGame6, ballGame, container, time);
                            }
                            foreach (var sitem in moreItemtr[1].FindElements(By.TagName("td")))
                            {
                                var iir = sitem.FindElement(By.TagName("p"));
                                string name = iir.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0];
                                string source = iir.GetAttribute("data-sp");
                                SESENT_BasketBallGame ballGame = new SESENT_BasketBallGame()
                                {
                                    BasketballID = Basketball,
                                    Code = RuleGenerateGameWithGameID.GetGameCode(No, dates[0], "9", ld++),
                                    Name = name,
                                    Source = source,
                                    Type = (int)PublicDefined.LqGageType.VictoryOrFailDiff_Main,
                                };
                                SESENT_BasketBallGame oldGame6 = container.SESENT_BasketBallGame.Where(k => k.Code == ballGame.Code).FirstOrDefault();
                                SaveOrUpdate(oldGame6, ballGame, container, time);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(No))
                    {
                        SaveChange(new Model.SESENT_BasketBallMatch()
                        {
                            Fk_FnID = RuleGenerateGame.Update_CacheID(F_Uid),
                            BasketballID =Basketball ,
                            MatchDate = DateTime.Parse(dates[0]),
                            MatchWeek = dates[1],
                            EndTime = Endtime,
                            Mainteam = Mainteam,
                            MainteamRanking = MainteamRanking,
                            No = No,
                            Visitingteam = Visitingteam,
                            VisitingteamRanking = VisitingteamRanking,
                            Match = Evt,
                        }, time - DateTime.Now);
                    }
                }
            }
        }
        public bool SaveChange(Model.SESENT_BasketBallMatch match, TimeSpan timeOut)
        {
            Model.ModelContainer container = new Model.ModelContainer();
            container.SESENT_BasketBallMatch.Add(match);
            SESENT_BasketBallMatch oldGame = container.SESENT_BasketBallMatch.Where(k => k.Fk_FnID == match.Fk_FnID).FirstOrDefault();
            if (oldGame == null)
            {
                container.SESENT_BasketBallMatch.Add(match);
                container.Entry(match).State = System.Data.Entity.EntityState.Added;
                bool isSuccess = container.SaveChanges() > 0;
                if (isSuccess)
                    Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_BasketBallMatch>(match.Fk_FnID.ToString()), match, timeOut);
                return isSuccess;
            }
            return true;
        }
        public bool IsExitsWithClass(IWebElement element, string className)
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
        public void SaveOrUpdate(Model.SESENT_BasketBallGame oldGame, Model.SESENT_BasketBallGame game, ModelContainer container, DateTime time)
        {
            if (oldGame != null)
            {
                oldGame.Name = game.Name;
                oldGame.Source = game.Source;
                container.Entry(oldGame).State = System.Data.Entity.EntityState.Modified;
                container.SaveChanges();
                Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_BasketBallGame>(game.Code.ToString()), oldGame, time - DateTime.Now);
            }
            else
            {
                container.SESENT_BasketBallGame.Add(game);
                container.Entry(game).State = System.Data.Entity.EntityState.Added;
                container.SaveChanges();
                Tools.RedisHelper.GetManger().SetWithList(Tools.CacheKey.GenerateCacheGameBall<SESENT_BasketBallGame>(game.Code.ToString()), game, time - DateTime.Now);
            }
        }
    }
}
