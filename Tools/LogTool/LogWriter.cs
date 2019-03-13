using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTool
{
    public static class LogWriter
    {
        private static log4net.ILog InfoLog;
        private static log4net.ILog ErrorLog;
        private static log4net.ILog debugLog;
        public static void InitConfigFile(string fileName)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(fileName));
            InfoLog = log4net.LogManager.GetLogger("loginfo");
            ErrorLog = log4net.LogManager.GetLogger("logerror");
            debugLog = log4net.LogManager.GetLogger("logdebug");
        }

        public static void WriteInfo(string msg)
        {
            if (InfoLog != null)
                InfoLog.Info(msg);
        }

        public static void WriteInfo(string msg, Exception ex)
        {
            if (InfoLog != null)
                InfoLog.Info(msg, ex);
        }

        public static void WriteDebug(string msg)
        {
            if (debugLog != null)
                debugLog.Debug(msg);
        }

        public static void WriteError(string error)
        {
            if (ErrorLog != null)
                ErrorLog.Error(error);
        }

        public static void WriteError(string msg, Exception ex)
        {
            if (ErrorLog != null)
            {
                ErrorLog.Error(msg, ex);
            }
        }
    }
}
