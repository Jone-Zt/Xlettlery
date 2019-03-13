using PublicDefined;
using System;
using System.Collections.Generic;
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
        bool QueryInbox(string AccountID,out IList<object> result,out string errMsg);
        /// <summary>
        /// 查询发件箱 ----------------发件箱和用户发送的均发送给后台的
        /// </summary>
        [OperationContract]
        bool QueryOutBox(string AccountID);
        [OperationContract]
        bool SendBox();
    }
}
