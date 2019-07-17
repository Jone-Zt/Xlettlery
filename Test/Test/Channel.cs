using ChannelInterface.ChannelInterface;
using ChannelInterFace;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Test
{
    public class Channel : TPayRechargeBase, IThridRecharge
    {
        public override void InitData(XmlNode paramets)
        {
           
        }
        public bool MakeOrder(string ChannelID, string OrderID, decimal amount, DateTime CreatOrder, out MakeOrderNewData result, out string errMsg)
        {
            errMsg = string.Empty;
            result = new MakeOrderNewData()
            {
                IsHtml = false,
                Result = "https://www.alipay.com/",
            };
            return true;
        }

        public override void Stop()
        {
          
        }
        public override bool Notify(string OrderID, string[] pathArges, Dictionary<string, string> UrlArges, byte[] postBuffer, out string BackStr,out decimal realPay)
        {
            realPay = 0;
            BackStr = "支付回调成功(Success)";
            return true;
        }
    }
}
