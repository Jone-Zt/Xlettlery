using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesInterface
{
    [ServiceContract]
    public interface IAuthorizeInterface
    {
        [OperationContract]
        bool CheckAuthorzeServices(string UserName, string UserPwd);
    }
}
