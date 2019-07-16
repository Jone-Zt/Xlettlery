﻿using Models;
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
            ResponsePicker<Model.MySlefGeneratePicker<object, Dictionary<string, object>>> picker = new ResponsePicker<Model.MySlefGeneratePicker<object, Dictionary<string, object>>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? lotteryId = RequestCheck.CheckIntValue(Request, "lotteryId", "足球支持编号", false);
                IlotteryInterface ilottery = GetManger();
                if (ilottery == null) throw new Exception("未挂载对应函数!");
                if (ilottery.QueryFootBallLottery((int)lotteryId, out List<Model.MySlefGeneratePicker<object, Dictionary<string, object>>> result, out string errMsg))
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
        public ActionResult QueryFootBallLotteryWithMeach()
        {
            ResponsePicker<Model.MySlefGeneratePicker<object, Dictionary<string, object>>> picker = new ResponsePicker<Model.MySlefGeneratePicker<object, Dictionary<string, object>>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? lotteryId = RequestCheck.CheckIntValue(Request, "lotteryId", "足球支持编号", false);
                int? type = RequestCheck.CheckIntValue(Request, "type", "足球玩法编号", false);
                IlotteryInterface ilottery = GetManger();
                if (ilottery == null) throw new Exception("未挂载对应函数!");
                if (ilottery.QueryFootBallLotteryWithMeach((int)lotteryId, (int)type, out List<Model.MySlefGeneratePicker<object, Dictionary<string, object>>> result, out string errMsg))
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
            ResponsePicker<Model.MySlefGeneratePicker<dynamic, Dictionary<string, object>>> picker = new ResponsePicker<Model.MySlefGeneratePicker<dynamic, Dictionary<string, object>>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? lotteryId = RequestCheck.CheckIntValue(Request, "lotteryId", "篮球支持编号", false);
                IlotteryInterface ilottery = GetManger();
                if (ilottery == null) throw new Exception("未挂载对应函数!");
                if (ilottery.QueryBasketBallLottery((int)lotteryId, out List<Model.MySlefGeneratePicker<dynamic, Dictionary<string, object>>> result, out string errMsg))
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

        public ActionResult MakeOrderWithFootBallGame()
        {
            ResponsePicker<object> picker = new ResponsePicker<object>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? lotteryId = RequestCheck.CheckIntValue(Request, "lotteryId", "足球支持编号", false);
                string AccountID = RequestCheck.CheckStringValue(Request, "AccountID", "用户编号", false);
                string Fids = RequestCheck.CheckStringValue(Request, "Fids", "游戏编号", false);
                string type = RequestCheck.CheckStringValue(Request, "type", "游戏类型", false);
                int? Multiple = RequestCheck.CheckIntValue(Request, "Multiple", "倍数", false);
                string[] parse = type.Split(',');
                int[] intArr = parse.Select(o => Convert.ToInt32(o)).ToArray<int>();
                IlotteryInterface ilottery = GetManger();
                if (ilottery == null) throw new Exception("未挂载对应函数!");
                if (!ilottery.MakeOrderWithFootBallGame(AccountID, (int)lotteryId, Fids, intArr, (int)Multiple, out object result, out string err))
                    picker.FailInfo = err;
                else
                    picker.Data = result;
            }
            catch (Exception err)
            {
                picker.FailInfo = err.Message;
            }
            return Content(picker.ToString());
        }

        public ActionResult QueryBasketBallLotteryWithType()
        {
            ResponsePicker<Model.MySlefGeneratePicker<dynamic, Dictionary<string, object>>> picker = new ResponsePicker<Model.MySlefGeneratePicker<dynamic, Dictionary<string, object>>>();
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
                if (ilottery.QueryBasketBallLotteryWithType((int)lotteryId, basketballID, (int)lqtype, out Model.MySlefGeneratePicker<dynamic, Dictionary<string, object>> result, out string errMsg))
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
            ResponsePicker<Model.MySlefGeneratePicker<dynamic, Dictionary<string, object>>> picker = new ResponsePicker<Model.MySlefGeneratePicker<dynamic, Dictionary<string, object>>>();
            try
            {
                string flowid = RequestCheck.CheckStringValue(Request, "flowID", "流水号", false);
                picker.FlowID = flowid;
                int? lotteryId = RequestCheck.CheckIntValue(Request, "lotteryId", "足球支持编号", false);
                long footballID = RequestCheck.CheckLongValue(Request, "footballID", "足球赛场编号", false);
                string strType = Request["Type"];
                int? Type = null;
                if (!string.IsNullOrEmpty(strType)) { Type = int.Parse(strType); }
                IlotteryInterface ilottery = GetManger();
                if (ilottery.QueryFootBallLotteryWithType((int)lotteryId, footballID, Type, out Model.MySlefGeneratePicker<dynamic, Dictionary<string, object>> result, out string errMsg))
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