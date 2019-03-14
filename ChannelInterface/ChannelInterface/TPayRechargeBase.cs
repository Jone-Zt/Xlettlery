using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChannelInterFace
{
    public abstract class TPayRechargeBase
    {
        public bool IsDebug { get; set; }
        public string AssemblyVersion { get; set; }
        public string ChannelTag { get; set; }
        public SESENT_ChannelProtocol MyProtocol { get; set; }
        public abstract void Stop();
        public abstract void InitData(XmlNode paramets);
    }
}
