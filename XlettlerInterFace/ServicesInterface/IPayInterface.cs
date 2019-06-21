using Model;
using PublicDefined;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServicesInterface
{
    [ServiceContract]
    public interface IPayInterface
    {
        [OperationContract]
        IList<SESENT_Channels> QueryChannels();
        [OperationContract]
        bool MakeOrder(decimal amount,string accountID,string channelID,out MakeOrderNewData result, out string errMsg);
        [OperationContract]
        Dictionary<string,object> QueryOrder(string accountID,DateTime OrderTime,int type,int pageIndex,int pageSize,out string errMsg);
        [OperationContract]
        bool CashWithdrawal(string accountID,decimal amount,int bankID,out Dictionary<string,object> result);
        IList<SESENT_CashCard> QueryCashBank(string accountID,out string errMsg);
        bool BindCashBank(SESENT_CashCard cashCard,out string errMsg);
    }
    [DataContract]
    public class MakeOrderNewData
    {
        [DataMember]
        public bool IsHtml { get; set; }
        [DataMember]
        public string Result { get; set; }
    }
}
