//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SESENT_BallOrder
    {
        public long OrderID { get; set; }
        public string FIds { get; set; }
        public short Status { get; set; }
        public System.DateTime EnterTime { get; set; }
        public string Type { get; set; }
        public string AccountID { get; set; }
        public int GameType { get; set; }
        public decimal Amount { get; set; }
        public Nullable<short> SettlementStatus { get; set; }
        public System.DateTime SettlementDate { get; set; }
    }
}
