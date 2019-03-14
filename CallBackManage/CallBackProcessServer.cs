using CallBackManage.CallBackHandle;
using HttpServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CallBackManage
{
    public class CallBackProcessServer : IlogTool
    {
        private Dictionary<string, ICallBackProcess> callProcess;
        private NewServer server;
        private int Port = 80;
        public CallBackProcessServer(int BindPort)
        {
            Port = BindPort;
            server = new NewServer(1000, 1024);
            FileCacheManage.GetManage().SetLogTool(this);
            server.SetLogTool(this);
            server.Init();
            server.SubscribeMsgReachEvent(new HttpServer.MsgReachEventDelegate(MsgReachEventHandle));

            //byte[] postData = DataConveter.DataConveter.HexStrToByte2In1(@"66 75 6E 64 5F 62 69 6C 6C 5F 6C 69 73 74 3D 25 35 42 25 37 42 25 32 32 61 6D 6F 75 6E 74 25 32 32 25 33 41 25 32 32 30 2E 30 31 25 32 32 25 32 43 25 32 32 66 75 6E 64 43 68 61 6E 6E 65 6C 25 32 32 25 33 41 25 32 32 41 4C 49 50 41 59 41 43 43 4F 55 4E 54 25 32 32 25 37 44 25 35 44 26 73 75 62 6A 65 63 74 3D 25 45 36 25 39 34 25 41 46 25 45 34 25 42 42 25 39 38 25 45 35 25 41 45 25 39 44 25 45 36 25 38 39 25 41 42 25 45 37 25 41 30 25 38 31 25 45 36 25 39 34 25 41 46 25 45 34 25 42 42 25 39 38 26 74 72 61 64 65 5F 6E 6F 3D 32 30 31 36 30 38 32 32 32 31 30 30 31 30 30 34 31 35 30 32 34 36 35 36 31 36 32 39 26 67 6D 74 5F 63 72 65 61 74 65 3D 32 30 31 36 2D 30 38 2D 32 32 2B 31 36 25 33 41 34 33 25 33 41 33 38 26 6E 6F 74 69 66 79 5F 74 79 70 65 3D 74 72 61 64 65 5F 73 74 61 74 75 73 5F 73 79 6E 63 26 74 6F 74 61 6C 5F 61 6D 6F 75 6E 74 3D 30 2E 30 31 26 6F 75 74 5F 74 72 61 64 65 5F 6E 6F 3D 30 37 30 45 33 36 38 30 30 30 30 31 26 69 6E 76 6F 69 63 65 5F 61 6D 6F 75 6E 74 3D 30 2E 30 31 26 6F 70 65 6E 5F 69 64 3D 32 30 38 38 30 30 33 36 34 33 30 30 33 36 35 37 37 30 39 38 33 32 31 31 39 31 35 31 30 33 31 35 26 73 65 6C 6C 65 72 5F 69 64 3D 32 30 38 38 32 32 31 37 31 33 35 38 34 30 33 32 26 6E 6F 74 69 66 79 5F 74 69 6D 65 3D 32 30 31 36 2D 30 38 2D 32 32 2B 31 36 25 33 41 34 37 25 33 41 30 34 26 74 72 61 64 65 5F 73 74 61 74 75 73 3D 54 52 41 44 45 5F 53 55 43 43 45 53 53 26 67 6D 74 5F 70 61 79 6D 65 6E 74 3D 32 30 31 36 2D 30 38 2D 32 32 2B 31 36 25 33 41 34 33 25 33 41 34 34 26 73 65 6C 6C 65 72 5F 65 6D 61 69 6C 3D 73 68 65 6E 6A 69 61 6A 69 6E 25 34 30 7A 68 75 6F 79 75 6E 6B 65 6A 69 2E 6E 65 74 26 72 65 63 65 69 70 74 5F 61 6D 6F 75 6E 74 3D 30 2E 30 31 26 62 75 79 65 72 5F 69 64 3D 32 30 38 38 30 32 32 37 32 32 35 39 33 31 35 34 26 61 70 70 5F 69 64 3D 32 30 31 36 30 37 31 35 30 31 36 32 33 30 36 36 26 6E 6F 74 69 66 79 5F 69 64 3D 66 61 64 64 61 33 30 30 63 39 34 35 31 35 39 61 31 65 65 33 34 32 65 31 61 37 36 66 35 38 61 68 35 71 26 62 75 79 65 72 5F 6C 6F 67 6F 6E 5F 69 64 3D 31 38 38 2A 2A 2A 2A 37 35 30 34 26 73 69 67 6E 5F 74 79 70 65 3D 52 53 41 26 62 75 79 65 72 5F 70 61 79 5F 61 6D 6F 75 6E 74 3D 30 2E 30 31 26 73 69 67 6E 3D 56 6C 49 55 38 52 39 53 43 6D 61 31 78 34 70 57 37 6D 46 34 57 25 32 42 4D 41 56 76 56 4C 72 57 68 4A 51 36 49 42 79 4D 52 72 7A 32 67 7A 68 37 36 7A 78 63 6A 30 52 6F 50 31 5A 34 62 70 52 62 77 7A 4A 36 47 6E 74 4C 50 4F 6D 6C 75 5A 62 31 63 61 71 70 66 41 75 41 73 30 49 32 58 59 59 42 64 53 73 4F 4A 73 6E 6D 59 44 6B 6B 47 5A 7A 32 37 44 31 42 79 35 37 38 6F 72 76 48 76 69 34 70 66 58 71 76 6C 70 53 55 5A 52 71 4F 52 36 4E 49 4F 34 76 65 31 33 37 50 75 78 63 42 64 6D 4E 74 61 6E 62 79 38 50 77 49 37 31 51 56 38 25 33 44 26 70 6F 69 6E 74 5F 61 6D 6F 75 6E 74 3D 30 2E 30 30");
            //OrderCallBackProcess.BusinessCallBack("ALIPAY", "070E36800002",null, null, postData);
        }

        public void InitHandles()
        {
            callProcess = new Dictionary<string, ICallBackProcess>();
            //订单动态回调
            ICallBackProcess aca = new OrderCallBackProcess();
            callProcess.Add(aca.GetProcessName(), aca);
        }

        public void Start()
        {
            server.Start(Port);
        }

        public bool MsgReachEventHandle(object sender, HttpRequest client)
        {
            string[] pathArges = client.RequestPath.Trim().Split('/');
            if (pathArges.Length > 0)
            {
                ICallBackProcess process = null;
                callProcess.TryGetValue(pathArges[1], out process);
                if (process != null)
                {
                    process.ProcessCallData(pathArges, client.RequestArges, client.MyContentBuffer, client.Response);
                }
            }
            return true;
        }

        public static string GetURLArgesStr(Dictionary<string, string> args)
        {
            if (args == null)
                return "";
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> vs in args)
            {
                sb.Append(vs.Key + "=" + vs.Value + "&");
            }
            return sb.ToString();
        }

        public static string GetPostArgesStr(byte[] args)
        {
            if (args == null)
                return "";
            return Encoding.UTF8.GetString(args,0, args.Length);
        }

        public void Stop()
        {
            server.Stop();
        }
        
        public void WriteError(string log, Exception ex)
        {
            LogTool.LogWriter.WriteError(log, ex);
        }

        public void WriteError(string log)
        {
            LogTool.LogWriter.WriteError(log);
        }

        public void WriteDebug(string msg)
        {
            LogTool.LogWriter.WriteDebug(msg);
        }

        public void WriteInfo(string msg, Exception ex)
        {
            LogTool.LogWriter.WriteInfo(msg, ex);
        }

        public void WriteInfo(string msg)
        {
            LogTool.LogWriter.WriteInfo(msg);
        }
    }
}
