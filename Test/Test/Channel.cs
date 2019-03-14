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

        public bool MakeOrder(decimal amount, DateTime CreatOrder, out MakeOrderNewData result, out string errMsg)
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
          
        }
    }
}
