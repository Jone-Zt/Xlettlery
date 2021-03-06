﻿using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesInterface
{
    [ServiceContract]
    public interface IlotteryInterface
    {
        [OperationContract]
        bool QuerySuportLottery(out IList<Model.SESENT_Lottery> result,out string errMsg);
        [OperationContract]
        bool QueryFootBallLotteryWithMeach(int lotteryId,int type, out Dictionary<string,List<MySlefGeneratePicker<object, Dictionary<string, object>>>> result, out string errMsg);
        [OperationContract]
        bool QueryFootBallLottery(int lotteryId, out Dictionary<string,List<MySlefGeneratePicker<object, Dictionary<string,object>>>> result, out string errMsg);
        [OperationContract]
        bool QueryFootBallLotteryWithType(int lotteryId, long FootBallID, int? type, out MySlefGeneratePicker<object, Dictionary<string, object>> result, out string errMsg);
        [OperationContract]
        bool QueryBasketBallLottery(int lotteryId, out Dictionary<string,List<MySlefGeneratePicker<object, Dictionary<string, object>>>> result, out string errMsg);
        [OperationContract]
        bool QueryBasketBallLotteryWithType(int lotteryId, long BaskBallID, int type, out MySlefGeneratePicker<object, Dictionary<string, object>> result, out string errMsg);
        [OperationContract]
        bool MakeOrderWithFootBallGame(string AccountID,int lotteryId, string Fids, int[] type, int Multiple,out object result,out string errMsg);
        [OperationContract]
        bool QueryOrderWithBall(string AccountID,bool Type,DateTime startTime,DateTime endTime,out DataTable result,out string errMsg);
        [OperationContract]
        bool QueryOrderWithBallDetail(string AccountID,int OrderID,out DataTable result,out string errMsg);
        [OperationContract]
        bool MakeUserFollow(string AccountID,string FollowID,int type,out string result,out string errMsg);
        [OperationContract]
        bool QueryUserFollow(string AccountID,out DataTable result,out string errMsg);
        [OperationContract]
        bool MakeGenDan(string AccountID, int lotteryId, int GameType, string Fids, int[] Games, int Multiple, out object result,out string errMsg);
    }
}
