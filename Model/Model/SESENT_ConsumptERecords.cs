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
    
    public partial class SESENT_ConsumptERecords
    {
        public int Id { get; set; }
        public string AccountID { get; set; }
        public decimal Withdrawable { get; set; }
        public string OrderID { get; set; }
        public System.DateTime EntryTime { get; set; }
        public short Status { get; set; }
        public decimal OutMoney { get; set; }
    }
}