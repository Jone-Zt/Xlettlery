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
    /// 提现流程
    /// </summary>
    public enum CashWithdrawalProcess
    {
        /// <summary>
        /// 未绑定手机
        /// </summary>
        NoBindRealName=100,
        /// <summary>
        /// 未绑定银行卡
        /// </summary>
        NoBindBankCard=200,
        /// <summary>
        /// 意外错误
        /// </summary>
        CashNoneFind=400,
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
        GradeAward,
        /// <summary>
        /// 用户提现
        /// </summary>
        CashWithdrow,
    }
}
