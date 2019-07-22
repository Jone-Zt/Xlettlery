using PublicDefined;
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
    public interface IShortMessageInterface
    {
        /// <summary>
        /// 查询收件箱
        /// </summary>
        [OperationContract]
        bool QueryInbox(string AccountID,out IList<Dictionary<string, DataTable>> result,out string errMsg);
        [OperationContract]
        bool InboxOperation(string AccountID,string MessageID,MessageStatus OperationType,out string errMsg);
    }
}
