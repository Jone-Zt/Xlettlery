using Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace XlettleryScrapy.ZqHandler
{
    public delegate void ZqKjHandler(string path);
    public class ZqKjScrapyHandler
    {
        private ZqKjScrapyHandler(string path,DateTime date)
        {
            string datetime = date.ToString("yyyy-MM-dd");
            path += datetime;
            this.date = DateTime.Parse(datetime);
            ZqListHandler = new ZqKjHandler(Init);
            ZqListHandler.Invoke(path);
            driver.Close();
        }
        private ZqKjHandler ZqListHandler;
        private ChromeDriver driver;
        private static ZqKjScrapyHandler handler = null;
        private DateTime date;
        private static object _sync = new object();
        public static ZqKjScrapyHandler GetHandler(string path,DateTime date)
        {
            if (handler == null)
            {
                lock (_sync)
                {
                    Interlocked.CompareExchange(ref handler, new ZqKjScrapyHandler(path,date), null);
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
            ModelContainer container = new ModelContainer();
            var atr=driver.FindElementByXPath("//*[@id='an_container']/div[1]/div[3]/div/table[1]").FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
            for (int i = 9; i < atr.Count; i++)
            {
               var atd=atr[i].FindElements(By.TagName("td"));
                string No = atd[0].Text;//编号
                string letball = atd[8].Text;//让球结果
                string letballMoney = atd[9].Text;//让球中奖金额
                string NotLetBall = atd[11].Text;//不让球结果
                string NotLetBallMoney = atd[12].Text;//不让球中奖金额
                string allInBall = atd[14].Text;//总进球数
                string allInBallMoney = atd[15].Text;//总进球数中奖金额
                string doubleResult = atd[17].Text;//半全场结果
                string doubleResultMoney = atd[18].Text;//半全场金额
                string parseResult = string.Empty;
                switch (doubleResult)
                {
                    case "胜胜":
                        parseResult = "3-3";
                        break;
                    case "胜平":
                        parseResult = "3-1";
                        break;
                    case "胜负":
                        parseResult = "3-0";
                        break;
                    case "平胜":
                        parseResult = "1-3";
                        break;
                    case "平平":
                        parseResult = "1-1";
                        break;
                    case "平负":
                        parseResult = "1-0";
                        break;
                    case "负胜":
                        parseResult = "0-3";
                        break;
                    case "负平":
                        parseResult = "0-1";
                        break;
                    case "负负":
                        parseResult = "0-0";
                        break;
                }
                SESENT_KJLottery lottery=container.SESENT_KJLottery.Where(a => a.LotteryTime == date && a.No == No).FirstOrDefault();
                if (lottery == null)
                {
                    List<SESENT_KJLottery> lotteries = new List<SESENT_KJLottery>();
                    SESENT_KJLottery LetBalllottery = new SESENT_KJLottery()
                    {
                        GameType = (int)PublicDefined.LetteryType.FootBall,
                        No = No,
                        LotteryMoney = decimal.Parse(letballMoney),
                        LotteryTime = date,
                        Source = letball,
                        Type = (int)PublicDefined.ZqGameType.Letball
                    };
                    SESENT_KJLottery NotBalllottery = new SESENT_KJLottery()
                    {
                        GameType=(int)PublicDefined.LetteryType.FootBall,
                        No = No,
                        LotteryMoney = decimal.Parse(NotLetBallMoney),
                        LotteryTime = date,
                        Source = NotLetBall,
                        Type = (int)PublicDefined.ZqGameType.NotLatball
                    };
                    SESENT_KJLottery AllInBalllottery = new SESENT_KJLottery()
                    {
                        GameType = (int)PublicDefined.LetteryType.FootBall,
                        No = No,
                        LotteryMoney = decimal.Parse(allInBallMoney),
                        LotteryTime = date,
                        Source = allInBall,
                        Type = (int)PublicDefined.ZqGameType.NumberofGoalsScored
                    };
                    SESENT_KJLottery DoubleResultlottery = new SESENT_KJLottery()
                    {
                        GameType = (int)PublicDefined.LetteryType.FootBall,
                        No = No,
                        LotteryMoney = decimal.Parse(doubleResultMoney),
                        LotteryTime = date,
                        Source = parseResult,
                        Type = (int)PublicDefined.ZqGameType.DoubleResult
                    };
                    lotteries.Add(LetBalllottery); lotteries.Add(NotBalllottery);lotteries.Add(AllInBalllottery); lotteries.Add(DoubleResultlottery);
                    container.SESENT_KJLottery.AddRange(lotteries);
                    container.SaveChanges();
                }
            }
                var ul = driver.FindElementByXPath("//*[@id='an_container']/div[1]/div[2]").FindElement(By.TagName("ul"));
                ul.FindElements(By.TagName("li"))[4].Click();
            var table = driver.FindElementByXPath("//*[@id='an_container']/div[1]/div[3]/div/table[1]/tbody");
            var trs = table.FindElements(By.TagName("tr"));
            for (int i = 1; i < trs.Count; i++)
            {
                var tds=trs[i].FindElements(By.TagName("td"));
                string No = tds[0].Text;
                string Name = tds[7].Text;
                string money = tds[9].Text;
                SESENT_KJLottery LetBalllottery = new SESENT_KJLottery()
                {
                    GameType = (int)PublicDefined.LetteryType.FootBall,
                    No = No,
                    LotteryMoney = decimal.Parse(money),
                    LotteryTime = date,
                    Source = Name,
                    Type = (int)PublicDefined.ZqGameType.Score
                };
                container.SESENT_KJLottery.Add(LetBalllottery);
                container.SaveChanges();
            }

        }
    }
}
