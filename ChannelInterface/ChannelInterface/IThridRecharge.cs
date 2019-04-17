using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelInterface.ChannelInterface
{
    public interface IThridRecharge
    {
        bool MakeOrder(string ChannelID, string OrderID, decimal amount,DateTime CreatOrder,out MakeOrderNewData result, out string errMsg);
    }
}
