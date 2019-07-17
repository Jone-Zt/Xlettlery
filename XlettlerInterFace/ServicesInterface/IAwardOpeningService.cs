using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesInterface
{
    [ServiceContract]
    public interface IAwardOpeningService
    {
        [OperationContract]
        bool GetFootBallAward(DateTime QueryTime,out List<dynamic> result,out string errMsg);
    }
}
