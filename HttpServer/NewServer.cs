using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HttpServer
{
    public delegate bool MsgReachEventDelegate(object sender, HttpRequest client);
    public class NewServer
    {
        private bool started = false;
        private int m_numConnections;
        private int m_receiveBufferSize;
        private Socket listenSocket;
        private SocketAsyncEventArgsPool m_readWritePool;
        private IlogTool logTool;
        public NewServer(int numConnections, int receiveBufferSize)
        {
            m_numConnections = numConnections;
            m_receiveBufferSize = receiveBufferSize;

            m_readWritePool = new SocketAsyncEventArgsPool(numConnections);
           
        }

        public void SetLogTool(IlogTool tool)
        {
            logTool = tool;
        }
        
        public void Init()
        {
            for (int i = 0; i < m_numConnections; i++)
            {
                HttpRequest request = CreateNewArg();
                request.IsTemp = false;
                m_readWritePool.Push(request);
            }
        }

        private HttpRequest CreateNewArg()
        {
            HttpRequest request = new HttpRequest(m_receiveBufferSize, m_receiveBufferSize * 5);

            SocketAsyncEventArgs readWriteEventArg = readWriteEventArg = new SocketAsyncEventArgs();
            readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
          
            request.SetLogTool(logTool);
            readWriteEventArg.UserToken = request;
            request.SocketArg = readWriteEventArg;
            return request;
        }

        public void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (listenSocket == null)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                logTool.WriteError(" listenSocket 是空的 "+ st.ToString());
                return;
            }
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                acceptEventArg.AcceptSocket = null;
            }
            try
            {
                bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
                if (!willRaiseEvent)
                {
                    ProcessAccept(acceptEventArg);
                }
            }
            catch (Exception ex)
            {
                logTool.WriteError("执行接收连接失败",ex);
            }
        }

        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            try
            {
                e.AcceptSocket.ReceiveTimeout = 5000;
                e.AcceptSocket.SendTimeout = 60000;
                HttpRequest request = m_readWritePool.Pop();
                if (request == null)
                {
                    logTool.WriteError("获取处理句柄失败 生成临时句柄");
                    request = CreateNewArg();
                    request.IsTemp = true;
                }
              
                request.SetClientSocket(e.AcceptSocket);
                request.SocketArg.AcceptSocket = e.AcceptSocket;
                request.SocketArg.SetBuffer(request.MyBuffer, 0, request.MyBuffer.Length);
                bool willRaiseEvent = e.AcceptSocket.ReceiveAsync(request.SocketArg);
                if (!willRaiseEvent)
                {
                    ProcessReceive(request.SocketArg);
                }
            }
            catch (Exception ex)
            {
                if (logTool != null)
                    logTool.WriteError("TCP SERVER Accept ERROR ", ex);
            }

            StartAccept(e);
        }

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                default:
                    throw new ArgumentException("操作类型错误。");
            }
        }
        

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            HttpRequest token = (HttpRequest)e.UserToken;
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (e.BytesTransferred > 0)
                    {
                        if (token.ParseRequestMsg(e.BytesTransferred))
                        {
                            ProcessRequest(token);
                        }
                    }
                }
                CloseClientSocket(token);
            }
            catch (Exception ex)
            {
                if (logTool != null)
                    logTool.WriteError("TCP SERVER Receive ERROR ", ex);
                CloseClientSocket(token);
            }
        }
        

        private void CloseClientSocket(HttpRequest e)
        {
            try
            {
                ShutDownSocket(e.SocketArg.AcceptSocket);
            }
            catch (Exception ex)
            {
                if (logTool != null)
                    logTool.WriteError("TCP SERVER CLOSE ERROR " + ex.Message, ex);
            }
            try
            {
                m_readWritePool.Push(e);
            }
            catch (Exception ex)
            {
                if (logTool != null)
                    logTool.WriteError("TCP SERVER Push ERROR " + ex.Message, ex);
            }
        }

        private void ShutDownSocket(Socket sok)
        {
            try
            {
                sok.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex) {
                if (logTool != null)
                    logTool.WriteError("TCP SERVER Shutdown ERROR " + ex.Message, ex);
            }
            try
            {
                sok.Close();
            }
            catch (Exception ex)
            {
                if (logTool != null)
                    logTool.WriteError("TCP SERVER Close Connect ERROR " + ex.Message, ex);
            }
        }

        public bool IsServerStart
        {
            get
            {
                return started;
            }
        }
        private event MsgReachEventDelegate eventMsgReach;
        public void SubscribeMsgReachEvent(MsgReachEventDelegate handler)
        {
            eventMsgReach += handler;
            return;
        }
        /*
         * WHAT:处理来自通讯模块的报文         
         */
        private void ProcessRequest(HttpRequest client)
        {
            try
            {
                if (eventMsgReach != null)
                {
                    eventMsgReach(this, client);
                }
            }
            catch (Exception ex)
            {
                if (logTool != null)
                    logTool.WriteError("TCP SERVER Process Request ERROR", ex);
            }
            return;
        }
        

        public void Start(int port)
        {
            if (!started)
            {
                started = true;
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listenSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                listenSocket.Listen(1000);
                StartAccept(null);
            }
        }

        public void Stop()
        {
            if (started)
            {
                started = false;
                try
                {
                    listenSocket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception) { }
                try
                {
                    listenSocket.Close();
                }
                catch (Exception)
                { }
                listenSocket = null;
            }
        }
    }
}
