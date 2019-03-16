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
        IList<SESENT_Order> QueryOrder(string accountID,DateTime OrderTime,int type,int pageIndex,int pageSize,out string errMsg);
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
