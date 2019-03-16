using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicDefined
{
    public enum OrderStatus
    {
        /// <summary>
        /// 支付成功
        /// </summary>
       success,
       /// <summary>
       /// 支付失败
       /// </summary>
       fail,
       /// <summary>
       /// 待支付
       /// </summary>
       wait,
    }
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 账户充值
        /// </summary>
        AccountRecharge,
        /// <summary>
        /// 账户消费
        /// </summary>
        AccountConsumption,
        /// <summary>
        /// 等级奖励
        /// </summary>
        GradeAward
    }
}
