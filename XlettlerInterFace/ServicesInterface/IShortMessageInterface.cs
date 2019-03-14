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
        bool QueryInbox(string AccountID,out IList<Dictionary<string, string>> result,out string errMsg);
    }
}
