using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesInterface
{
    [ServiceContract]
    public interface IMessageInterface
    {
        [OperationContract]
        bool QueryInformation(int ID,out Model.SESENT_InfoMation result, out string errMsg);
        [OperationContract]
        bool QueryInformationWithList(out IList<Dictionary<string,object>> valuePairs,out string errMsg);
    }
}
