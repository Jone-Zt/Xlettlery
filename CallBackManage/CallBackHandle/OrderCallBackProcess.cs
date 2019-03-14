using System;
using System.Collections.Generic;
using System.Text;
using HttpServer;
using ServicesInterface;
using Tools;

namespace CallBackManage.CallBackHandle
{
    class OrderCallBackProcess : ICallBackProcess
    {
        public string GetProcessName()
        {
            return "Order";
        }

        public void ProcessCallData(string[] pathArges, Dictionary<string, string> UrlArges, byte[] postBuffer, HttpResponse response)
        {
            if (pathArges == null || pathArges.Length < 4)
                return;
            try
            {
                string channelID = pathArges[2];
                string orderID = pathArges[3];
                string[] customerArges = null;
                if (pathArges.Length > 4)
                {
                    customerArges = new string[pathArges.Length - 4];
                    Array.Copy(pathArges, 4, customerArges, 0, customerArges.Length);
                }
                string BackStr = "";
                IChannelNotify proxy = RemotingAngency.GetRemoting().GetProxy<IChannelNotify>();
                if (proxy != null) {
                    proxy.Notify(orderID, channelID, pathArges, UrlArges, postBuffer, out BackStr);
                }
                LogTool.LogWriter.WriteDebug("订单 回调信息处理 url:" + string.Join("/", pathArges) + " Get:" + CallBackProcessServer.GetURLArgesStr(UrlArges) + "\r\nPost:" +
                   CallBackProcessServer.GetPostArgesStr(postBuffer)+"\r\n响应消息:"+ BackStr);
                response.SendResponseHTML(BackStr);
            }
            catch (Exception ex)
            {
                LogTool.LogWriter.WriteError("订单 回调信息处理失败 url:" + string.Join("/", pathArges) + " Get:" + CallBackProcessServer.GetURLArgesStr(UrlArges)+"\r\nPost:"+ 
                    CallBackProcessServer.GetPostArgesStr(postBuffer), ex);
                response.SendContent(HttpRequestErrorCode.NotFound, null, "UTF-8", HttpContentType.TextHtml);
            }
        }
    }
}
