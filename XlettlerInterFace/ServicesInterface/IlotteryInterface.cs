using Model;
using System;
using System.Collections.Generic;
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
        bool QueryFootBallLottery(int lotteryId, out List<MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch>> result, out string errMsg);
        [OperationContract]
        bool QueryFootBallLotteryWithType(int lotteryId, long FootBallID, int? type, out MySlefGeneratePicker<Model.SESENT_FootBallGame, Model.SESENT_FootBallMatch> result, out string errMsg);
        [OperationContract]
        bool QueryBasketBallLottery(int lotteryId, out List<MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch>> result, out string errMsg);
        [OperationContract]
        bool QueryBasketBallLotteryWithType(int lotteryId, long BaskBallID, int type, out MySlefGeneratePicker<Model.SESENT_BasketBallGame, Model.SESENT_BasketBallMatch> result, out string errMsg);
        /// <summary>
        /// 下单接口
        /// </summary>
        /// <param name="lotteryId">cp编号</param>
        /// <param name="MainID">赛场编号</param>
        /// <param name="Fids">玩法数据</param>
        /// <param name="type">2串1:2 </param>
        /// <param name="Multiple">倍数</param>
        /// <returns></returns>
        [OperationContract]
        bool MakeOrderWithFootBallGame(string AccountID,int lotteryId, string Fids, int type, int Multiple,out object result,out string errMsg);
    }
}
