using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer
{
    public interface IlogTool
    {
        void WriteError(string log, Exception ex);
        void WriteError(string log);
        void WriteDebug(string msg);
        void WriteInfo(string msg, Exception ex);
        void WriteInfo(string msg);
    }
}
