using Models;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tools;

namespace MobileAPI.Controllers
{
    public class LotteryController : Controller
    {
        private IlotteryInterface GetManger()
        {
            IlotteryInterface proxy = null;
            if (proxy == null)
            {
                proxy = RemotingAngency.GetRemoting().GetProxy<IlotteryInterface>();
            }
            return proxy;
        }


        public ActionResult QuerySupportLottery()
        {
            ResponsePicker<Model.SESENT_Lottery> picker = new ResponsePicker<Model.SESENT_Lottery>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                IlotteryInterface ilottery = GetManger();
                if (ilottery == null) throw new Exception("未挂载对应函数!");
                if (ilottery.QuerySuportLottery(out IList<Model.SESENT_Lottery> result, out string errMsg))
                    picker.List = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult QueryFootBallLottery()
        {
            ResponsePicker<Model.MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch>> picker = new ResponsePicker<Model.MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? lotteryId = RequestCheck.CheckIntValue(Request, "lotteryId", "足球支持编号", false);
                IlotteryInterface ilottery = GetManger();
                if (ilottery == null) throw new Exception("未挂载对应函数!");
                if (ilottery.QueryFootBallLottery((int)lotteryId, out List<Model.MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch>> result, out string errMsg))
                    picker.List = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult QueryBasketBallLottery()
        {
            ResponsePicker<Model.MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch>> picker = new ResponsePicker<Model.MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? lotteryId = RequestCheck.CheckIntValue(Request, "lotteryId", "篮球支持编号", false);
                IlotteryInterface ilottery = GetManger();
                if (ilottery == null) throw new Exception("未挂载对应函数!");
                if (ilottery.QueryBasketBallLottery((int)lotteryId, out List<Model.MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch>> result, out string errMsg))
                    picker.List = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult QueryBasketBallLotteryWithType()
        {
            ResponsePicker<Model.MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch>> picker = new ResponsePicker<Model.MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? lotteryId = RequestCheck.CheckIntValue(Request, "lotteryId", "篮球支持编号", false);
                long basketballID = RequestCheck.CheckLongValue(Request, "basketballID", "篮球赛场编号", false);
                int? Type = RequestCheck.CheckIntValue(Request, "Type", "玩法类型", false);
                PublicDefined.LqGageType lqtype = (PublicDefined.LqGageType)Type;
                IlotteryInterface ilottery = GetManger();
                if (ilottery == null) throw new Exception("未挂载对应函数!");
                if (ilottery.QueryBasketBallLotteryWithType((int)lotteryId, basketballID,(int)lqtype, out Model.MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch> result, out string errMsg))
                    picker.Data = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
        public ActionResult QueryFootBallLotteryWithType()
        {
            ResponsePicker<Model.MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch>> picker = new ResponsePicker<Model.MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? lotteryId = RequestCheck.CheckIntValue(Request, "lotteryId", "足球支持编号", false);
                long footballID = RequestCheck.CheckLongValue(Request, "footballID", "足球赛场编号", false);
                string strType=Request["Type"];
                int? Type = null;
                if (!string.IsNullOrEmpty(strType)) { Type = int.Parse(strType);}
                IlotteryInterface ilottery = GetManger();
                if (ilottery.QueryFootBallLotteryWithType((int)lotteryId, footballID, Type, out Model.MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch> result, out string errMsg))
                    picker.Data = result;
                else
                    picker.FailInfo = errMsg;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }
    }
}