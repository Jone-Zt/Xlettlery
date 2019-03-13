using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PublicDefined
{
    [DataContract]
    public enum SettingType
    {
        /// <summary>
        /// 轮播图
        /// </summary>
        [EnumMember]
        Advertisement =1,
        /// <summary>
        /// 引导页
        /// </summary>
        [EnumMember]
        Guide =2
    }
}
