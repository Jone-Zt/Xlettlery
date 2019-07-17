using ChannelInterface.ChannelInterface;
using ChannelInterFace;
using Newtonsoft.Json;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Test.Models;
using Tools;

namespace Test
{
    public class Channel : TPayRechargeBase, IThridRecharge
    {
        private string mchId { get { return "10018";} }
        private string appId { get { return "ab8de587ca2f4cea97137f1b9d442835"; } }
        private string key { get { return "VPXSXWSO6A2Y2W1BN3VE3GMLJZUQ4V3ONMIUINQBAVHYF989CMWTGGDWXYPWQT2DHCBWD0HTBUF5C3KYXJH3RHPZOTG1XVSBQ7WT1UFOS47L4ES3QLUVKUKFTSQNKUO2"; } }
        private string PayUrl { get { return "http://api.cokapay.com/api/pay/create_payorder"; } }
        public override void InitData(XmlNode paramets)
        {
           
        }
        public bool MakeOrder(string ChannelID, string OrderID, decimal amount, DateTime CreatOrder, out MakeOrderNewData result, out string errMsg)
        {
            errMsg = string.Empty; result = null;
            try
            {
                SortedDictionary<string, object> valuePairs = new SortedDictionary<string, object>();
                valuePairs.Add("mchId", mchId);valuePairs.Add("appId", appId);valuePairs.Add("productId", "8037");
                valuePairs.Add("mchOrderNo", OrderID);valuePairs.Add("currency", "cny");valuePairs.Add("amount", (int)(amount*100));
                valuePairs.Add("clientIp", "210.73.10.148");valuePairs.Add("notifyUrl",GetNotifyUrl(ChannelID,OrderID));
                valuePairs.Add("subject", "充值");valuePairs.Add("body", "MC、祝君中奖!");
                StringBuilder builder = new StringBuilder();
                builder.Append("amount=").Append(valuePairs["amount"] ?? "").Append("&");
                builder.Append("appId=").Append(valuePairs["appId"] ?? "").Append("&");
                builder.Append("body=").Append(valuePairs["body"] ?? "").Append("&");
                builder.Append("clientIp=").Append(valuePairs["clientIp"] ?? "").Append("&");
                builder.Append("currency=").Append(valuePairs["currency"] ?? "").Append("&");
                builder.Append("mchId=").Append(valuePairs["mchId"] ?? "").Append("&");
                builder.Append("mchOrderNo=").Append(valuePairs["mchOrderNo"] ?? "").Append("&");
                builder.Append("notifyUrl=").Append(valuePairs["notifyUrl"] ?? "").Append("&");
                builder.Append("productId=").Append(valuePairs["productId"] ?? "").Append("&");
                builder.Append("subject=").Append(valuePairs["subject"] ?? "").Append("&");
                builder.Append("key=").Append( key?? "");
                string signData= MD5Helper.GetMd5Str(builder.ToString());
                valuePairs.Add("sign",signData);
                string resultpicker=HttpHelper.SendRequest(PayUrl, valuePairs, MethordType.POST,Encoding.UTF8);
                LogTool.LogWriter.WriteDebug($"支付信息:{resultpicker}");
                if (!string.IsNullOrEmpty(resultpicker))
                {
                    MakeOrderPicker obj =JsonConvert.DeserializeObject<MakeOrderPicker>(resultpicker);
                    if (obj != null && obj.retCode.ToUpper()== "SUCCESS")
                    {
                        result = new MakeOrderNewData()
                        {
                            IsHtml = false,
                            Result = obj.payParams.payUrl,
                        };
                        return true;
                    }
                }
                errMsg = resultpicker;
                return false;
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError("MakeOrder FAIL:"+err.Message);
                return false;
            }
        }

        public override void Stop()
        {
          
        }
        public override bool Notify(string OrderID, string[] pathArges, Dictionary<string, string> UrlArges, byte[] postBuffer, out string BackStr,out decimal realPay)
        {
            realPay = 0;
            LogTool.LogWriter.WriteError($"回掉信息:{Encoding.UTF8.GetString(postBuffer)}");
            BackStr = "支付回调成功(Success)";
            return true;
        }
    }
}
