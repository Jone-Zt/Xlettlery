using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HttpServer
{
    class SocketAsyncEventArgsPool
    {
        private readonly Stack<HttpRequest> m_pool;
        private readonly object syncLocker = new object();
        Semaphore m_maxNumberAcceptedClients;
        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<HttpRequest>(capacity);
            m_maxNumberAcceptedClients = new Semaphore(0, capacity);
        }

        public void Push(HttpRequest item)
        {
            if (item == null)
            {
                return;
            }
            if (item.IsTemp)
            {
                item.SocketArg.UserToken = null;
                item.SocketArg = null;
                return;
            }
            m_pool.Push(item);
            m_maxNumberAcceptedClients.Release();
        }

        public HttpRequest Pop()
        {
            if (m_maxNumberAcceptedClients.WaitOne(2000))
            {
                return m_pool.Pop();
            }
            else
            {
                return null;
            }
        }

        public int Count { get { return m_pool.Count; } }
    }
}
