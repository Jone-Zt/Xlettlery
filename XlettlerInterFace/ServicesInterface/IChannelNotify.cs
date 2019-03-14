using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicesInterface
{
    [ServiceContract]
    public interface IChannelNotify
    {
        [OperationContract]
        bool Notify(string OrderID,string ChannelID, string[] pathArges, Dictionary<string, string> UrlArges, byte[] postBuffer,out string BackStr);
    }
}
