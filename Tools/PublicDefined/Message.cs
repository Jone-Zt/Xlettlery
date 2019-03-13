using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PublicDefined
{
    [DataContract]
    public enum MessageObjType
    {
        //群发
        [EnumMember]
        All,
        //点对点
        [EnumMember]
        Singler
    }
    [DataContract]
    public enum MessageStatus
    {
        /// <summary>
        /// 未阅读
        /// </summary>
        [EnumMember]
        Unreaded,
        /// <summary>
        /// 已阅
        /// </summary>
        [EnumMember]
        Readed,
        /// <summary>
        /// 未领取
        /// </summary>
        [EnumMember]
        Uncollected,
        /// <summary>
        /// 已领取
        /// </summary>
        [EnumMember]
        Collected
    }
}
