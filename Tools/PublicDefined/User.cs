using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PublicDefined
{
    /// <summary>
    /// 账户类型
    /// </summary>
    [DataContract]
    public enum UserType
    {
        /// <summary>
        /// 代理会员
        /// </summary>
        [EnumMember]
        angency =8888,
        /// <summary>
        /// 普通会员
        /// </summary>
        [EnumMember]
        member =2222,
    }
    /// <summary>
    /// 手机验证码类型
    /// </summary>
    [DataContract]
    public enum IPhoneCodeType
    {
        /// <summary>
        /// 登陆
        /// </summary>
        [EnumMember]
        Login =1,
        /// <summary>
        /// 注册
        /// </summary>
        [EnumMember]
        Register =2,
        /// <summary>
        /// 修改登陆密码验证码
        /// </summary>
        [EnumMember]
        FindLoginPwd =3,
        /// <summary>
        /// 绑定银行卡
        /// </summary>
        [EnumMember]
        BindBankCard=4,
    }
    [DataContract]
    public enum Status
    {
        [EnumMember]
        Open,
        [EnumMember]
        Close,
    }
    [DataContract]
    public enum LoginType
    {
        /// <summary>
        /// 账户登陆
        /// </summary>
        [EnumMember]
        Account =1,
        /// <summary>
        /// 手机登陆
        /// </summary>
        [EnumMember]
        PhoneCode =2
    }
}
