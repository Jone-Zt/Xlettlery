using ChannelInterface.ChannelInterface;
using ChannelInterFace;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Test
{
    public class Channel : TPayRechargeBase, IThridRecharge
    {
        public override void InitData(XmlNode paramets)
        {
           
        }

        public bool MakeOrder(string ChannelID,string OrderID,decimal amount, DateTime CreatOrder, out MakeOrderNewData result, out string errMsg)
        {
            errMsg = string.Empty;
            result = new MakeOrderNewData();
            result.IsHtml = false;
            result.Result = GetNotifyUrl(ChannelID, OrderID);
            return true;
        }

        public override void Stop()
        {
          
        }
    }
}
