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
    
    public partial class SESENT_Channels
    {
        public SESENT_Channels()
        {
            this.Status = 0;
            this.LimitMin = 0;
            this.LimitMax = 99999;
        }
    
        public int Id { get; set; }
        public string ChannelID { get; set; }
        public string ChannelName { get; set; }
        public short Status { get; set; }
        public int LimitMin { get; set; }
        public int LimitMax { get; set; }
        public short ChannelType { get; set; }
    }
}
