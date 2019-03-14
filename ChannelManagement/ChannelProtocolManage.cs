using ChannelInterFace;
using Model;
using PublicDefined;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace ChannelManagement
{
    public class ChannelProtocolManage
    {
        private static ChannelProtocolManage Managment;
        private static object lockObj = new object();
        public static ChannelProtocolManage GetManagment()
        {
            if (Managment == null)
            {
                lock (lockObj)
                {
                    Interlocked.CompareExchange(ref Managment, new ChannelProtocolManage(),null);
                }
            }
            return Managment;
        }
        private Dictionary<string, TPayRechargeBase> PayInterfaces = null;
        private Dictionary<string, System.Reflection.Assembly> dllFileData;
        public string Init()
        {
            string msg = "";
            StopChannel();
            Dictionary<string, TPayRechargeBase> Pays = new Dictionary<string, TPayRechargeBase>();
            bool complete = true;
            try
            {
                using (ModelContainer container = new ModelContainer()) 
                {
                    short status =(short)Status.Open;
                    List<SESENT_ChannelProtocol> protocols = container.SESENT_ChannelProtocol.Where(a => a.Status == status).ToList();
                    if (protocols != null && protocols.Count > 0)
                    {
                        foreach (SESENT_ChannelProtocol protocol in protocols)
                        {
                            TPayRechargeBase tcpb = null;
                            if (PayInterfaces != null)
                            {
                                if (PayInterfaces.ContainsKey(protocol.ProtocolID))
                                    tcpb = PayInterfaces[protocol.ProtocolID];
                            }
                            tcpb = CreateRechChannel(protocol, tcpb);
                            if (tcpb != null)
                            {
                                Pays[protocol.ProtocolID] = tcpb;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.LogWriter.WriteError("通道协议初始化失败 ", ex);
                msg = "通道协议初始化失败 " + ex.Message;
                complete = false;
            }
            if (complete || PayInterfaces == null)
            {
                PayInterfaces = Pays;
            }
            return msg;
        }
        public void StopChannel()
        {
            if (PayInterfaces != null)
            {
                foreach (TPayRechargeBase tpr in PayInterfaces.Values)
                {
                    try
                    {
                        tpr.Stop();
                    }
                    catch (Exception ex)
                    {
                        LogTool.LogWriter.WriteError("通道服务停止错误 ", ex);
                    }

                }
            }
        }
        private TPayRechargeBase CreateRechChannel(SESENT_ChannelProtocol channel, TPayRechargeBase tcpb)
        {
            if (channel.ConfigFile == null)
            {
                LogTool.LogWriter.WriteInfo("通道协议" + channel.ProtocolName + " 缺少配置文件。");
                return null;
            }
            //初始化通道配置数据
            XmlDocument doc = new XmlDocument();
            string ChannelClass = "";
            XmlNode parameterNode = null;
            bool isDebug = false;
            string channelTag = "";
            string assName = null;
            bool autoQuery = false;
            string xmlStr = Encoding.UTF8.GetString(channel.ConfigFile);
            int index = xmlStr.IndexOf("?>");
            if (index > 0)
            {
                xmlStr = xmlStr.Substring(index + 2);
            }
            if (string.IsNullOrEmpty(xmlStr))
            {
                LogTool.LogWriter.WriteError("通道协议 " + channel.ProtocolName + " 配置文件解密失败。");
                return null;
            }
            try
            {
                doc.LoadXml(xmlStr);
                foreach (XmlNode root in doc.ChildNodes)
                {
                    if (root.Name == "Config")
                    {
                        XmlAttribute configAtt = root.Attributes["IsDebug"];
                        if (configAtt != null)
                        {
                            isDebug = configAtt.Value == "1";
                        }
                        configAtt = root.Attributes["AutoQuery"];
                        if (configAtt != null)
                        {
                            autoQuery = configAtt.Value == "1";
                        }
                        configAtt = root.Attributes["ChannelTag"];
                        if (configAtt != null)
                        {
                            channelTag = configAtt.Value;
                        }
                        foreach (XmlNode node in root.ChildNodes)
                        {
                            switch (node.Name)
                            {
                                case "ChannelClass":
                                    ChannelClass = node.Attributes["Value"].Value;
                                    if (node.Attributes["Assembly"] != null)
                                        assName = node.Attributes["Assembly"].Value;
                                    break;
                                case "Parameters":
                                    parameterNode = node;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogTool.LogWriter.WriteError("通道协议 " + channel.ProtocolName + " 配置文件解析出错。XML:" + xmlStr, ex);
                return null;
            }
            if (string.IsNullOrEmpty(channelTag))
                channelTag = ChannelClass;
            object obj = null;
            string AssemblyVersion = "";

            if (!string.IsNullOrEmpty(assName))
            {
                try
                {
                    System.Reflection.Assembly ably = GetOldFileAssembly(assName);
                    if (ably == null)
                    {
                        LogTool.LogWriter.WriteError("通道协议 " + channel.ProtocolName + " 获取程序集信息失败 AssemblyName:" + assName);
                    }
                    AssemblyVersion = ably.GetName().Version.ToString();
                    obj = ably.CreateInstance(ChannelClass);
                }
                catch (Exception ex)
                {
                    LogTool.LogWriter.WriteError("通道协议 " + channel.ProtocolName + " 获取通道对象类型失败。" + ChannelClass + " AssemblyName:" + assName, ex);
                    return null;
                }
            }
            if (obj == null)
            {
                Type channelType = Type.GetType(ChannelClass);
                if (channelType == null)
                {
                    LogTool.LogWriter.WriteError("通道协议 " + channel.ProtocolName + " 获取通道对象类型失败。" + ChannelClass);
                    return null;
                }
                obj = Activator.CreateInstance(channelType);
            }
            if (obj == null)
            {
                LogTool.LogWriter.WriteError("通道协议 " + channel.ProtocolName + " 创建对象失败。");
                return null;
            }
            TPayRechargeBase tpBase = null;
            if (tcpb != null && tcpb.AssemblyVersion == AssemblyVersion)
            {
                tpBase = tcpb;
            }
            else
            {
                if (tcpb != null)
                    tcpb.Stop();
                tpBase = obj as TPayRechargeBase;
            }
            if (tpBase == null)
            {
                LogTool.LogWriter.WriteError("通道协议 " + channel.ProtocolName + " 创建对象类型不匹配。");
                return null;
            }
            tpBase.AssemblyVersion = AssemblyVersion;
            tpBase.IsDebug = isDebug;
            tpBase.MyProtocol = channel;
            try
            {
                tpBase.InitData(parameterNode);
            }
            catch (Exception ex)
            {
                LogTool.LogWriter.WriteError("通道协议 " + channel.ProtocolName + " 通道初始化失败。", ex);
                return null;
            }
            tpBase.ChannelTag = channelTag;
            return tpBase;
        }
        public System.Reflection.Assembly GetOldFileAssembly(string FileName)
        {
            if (dllFileData == null)
                dllFileData = new Dictionary<string, System.Reflection.Assembly>();
            byte[] buffer = System.IO.File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\" + FileName);
            System.Reflection.Assembly nowAss = System.Reflection.Assembly.Load(buffer);
            if (nowAss == null)
                return null;
            if (dllFileData.ContainsKey(FileName))
            {
                System.Reflection.Assembly tempAss = dllFileData[FileName];
                if (tempAss.GetName().Version.ToString() != nowAss.GetName().Version.ToString())
                    dllFileData[FileName] = nowAss;
                else
                    nowAss = tempAss;
                return nowAss;
            }
            else
            {
                dllFileData[FileName] = nowAss;
                return nowAss;
            }
        }
    }
}
