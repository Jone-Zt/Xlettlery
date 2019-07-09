using PublicDefined;
using System.Collections.Generic;
using System.ServiceModel;

namespace ServicesInterface
{
    [ServiceContract]
    public interface IUserInterface
    {
        [OperationContract]
        bool Register(string AccountID, string passWord,string Phone,string agencyID,string Code,UserType type,out string errMsg);
        [OperationContract]
        bool Login(string AccountID, string passWord,string Phone,string Code, LoginType type,out string errMsg);
        [OperationContract]
        bool SendUserCode(string Phone, IPhoneCodeType type,out string errMsg);
        [OperationContract]
        bool QueryUserInfo(string userName,out IDictionary<string, object> result,out string errMsg);
        [OperationContract]
        bool FindLoginPwd(string Phone,string Code,string passWord,out string errMsg);
        [OperationContract]
        bool CheckReister(string Phone,string AccountID,out string Msg);
        [OperationContract]
        bool BindRealName(string AccountID,string RealName,string IdCardNum,out string result,out string errMsg);
    }
}
