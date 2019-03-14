using HttpServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CallBackManage
{
    public interface ICallBackProcess
    {
        string GetProcessName();
        void ProcessCallData(string[] pathArges, Dictionary<string, string> UrlArges, byte[] postBuffer, HttpResponse response);
    }
}
