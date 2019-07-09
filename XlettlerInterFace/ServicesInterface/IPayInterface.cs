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
        QueryOrderPicker QueryOrder(string accountID,DateTime BeginTime,DateTime EndTime,int type,int pageIndex,int pageSize,out string errMsg);
        [OperationContract]
        Dictionary<string, object> CashWithdrawal(string accountID,decimal amount,int bankID);
        [OperationContract]
        bool QueryCashBank(string accountID,out IList<SESENT_CashCard> result,out string errMsg);
        [OperationContract]
        bool BindCashBank(SESENT_CashCard cashCard,out string result,out string errMsg);
        [OperationContract]
        bool QueryNetSite(string Province, string City, string BankName,string NiteSite, out SESENT_BankLineNumber[] result,out string errMsg);
        [OperationContract]
        bool QueryBankName(out IList<string> result, out string errMsg);
    }
    [DataContract]
    public class MakeOrderNewData
    {
        [DataMember]
        public bool IsHtml { get; set; }
        [DataMember]
        public string Result { get; set; }
    }
    [DataContract]
    public class QueryOrderPicker
    {
        [DataMember]
        public int PageNums { get; set; }
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public List<SESENT_Order> Orders { get; set; }
    }
}
