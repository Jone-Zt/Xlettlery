using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesInterface
{
    interface IChannelNotify
    {
        bool Notify(string OrderID,string ChannelID);
    }
}
