using Model;
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
    public interface ISettingInterface
    {
        [OperationContract]
        bool Settingpage(SettingType type, out List<SESENT_Settings> result,out string errMsg);
    }
}
