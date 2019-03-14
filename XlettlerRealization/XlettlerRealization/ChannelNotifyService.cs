using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XlettlerRealization
{
    public class ChannelNotifyService : IChannelNotify
    {
        public bool Notify(string OrderID, string ChannelID, string[] pathArges, Dictionary<string, string> UrlArges, byte[] postBuffer, out string BackStr)
        {
            throw new NotImplementedException();
        }
    }
}
