using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Model;
using PublicDefined;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tools
{
    [Serializable]
    public class MQData
    {
        public string Phone { get; set; }
        public IPhoneCodeType Type { get; set; }
        public string Content { get; set; }
        public TimeSpan Life { get; set; }
        public string Code { get; set; }
    }
    public class ActiveMQHelper
    {
        private StringBuilder _connStr=new StringBuilder();
        private IConnectionFactory _factory;
        private ActiveMQHelper() {
            try
            {
                string ip = System.Configuration.ConfigurationManager.AppSettings["ActiveMQServer"];
                if (!string.IsNullOrEmpty(ip)) {
                    _connStr.Append("tcp://").Append(ip).Append("?wireFormat.maxInactivityDuration=0");
                }
                _factory = new ConnectionFactory(_connStr.ToString()?? "tcp://127.0.0.1:61616");
                Start();
            }
            catch(Exception err)
            {
                LogTool.LogWriter.WriteError("ActiveMQ初始化失败!"+err.Message);
            }
        }
        private static readonly object _sync = new object();
        private static ActiveMQHelper _activeMQ;
        public static ActiveMQHelper GetManger()
        {
            if (_activeMQ == null) {
                lock (_sync) {
                    Interlocked.CompareExchange(ref _activeMQ,new ActiveMQHelper(),null);
                }
            }
            return _activeMQ;
        }
        private void Start()
        {
           IConnection connection=_factory.CreateConnection();
           connection.ClientId = "Server";
           connection.Start();
           ISession session = connection.CreateSession();
           IMessageConsumer consumer=session.CreateConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQQueue("phoneCode"), "filter='demo'");
           consumer.Listener += Consumer_Listener;
        }

        private void Consumer_Listener(IMessage message)
        {
            IObjectMessage textMessage = (IObjectMessage)message;
            dynamic obj = textMessage.Body;
            StringBuilder builder = new StringBuilder(obj.Content);
            builder.Replace("{user}",obj.Phone);
            builder.Replace("{code}", obj.Code);
            builder.Replace("{life}",((Math.Round(((TimeSpan)obj.Life).TotalMinutes))).ToString());
            LogTool.LogWriter.WriteDebug($"短信通道请求:{builder.ToString()}");
            LogTool.LogWriter.WriteDebug("短信通道响应:"+Devicer.SendMsg(obj.Phone,builder.ToString()));
        }
        public bool SendMessage(string phone, IPhoneCodeType type)
        {
            try
            {
                using (IConnection connection = _factory.CreateConnection())
                {
                    using (ISession session = connection.CreateSession())
                    {
                        IMessageProducer prod = session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQQueue("phoneCode"));
                        ModelContainer dbm = new ModelContainer();
                        short typeID = short.Parse(((int)type).ToString());
                        Model.SESENT_PhoneXCodes xCodes = dbm.SESENT_PhoneXCodes.Where(a => a.Type == typeID).FirstOrDefault();
                        if (xCodes != null)
                        {
                            DateTime dateNow = DateTime.Now;
                            TimeSpan timeOut = dateNow.AddMinutes(xCodes.Life) - dateNow;
                            MQData mQData = new MQData() { Phone = phone, Content = xCodes.Content, Code = CacheKey.GenerateRandomStr(8), Type = type, Life = timeOut };
                            IObjectMessage message = prod.CreateObjectMessage(mQData);
                            message.Properties.SetString("filter", "demo");
                            RedisHelper.GetManger().Set(CacheKey.GenerateCachePhoneCode(phone, type), mQData.Code, mQData.Life);
                            prod.Send(message, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
                            return true;
                        }
                        else
                        {
                            LogTool.LogWriter.WriteError($"未查询到短信模板【短信模板编号:{type}】");
                            return false;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError("短信发送失败",err);
                return false;
            }
        }
    }
}
