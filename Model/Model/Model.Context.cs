﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ModelContainer : DbContext
    {
        public ModelContainer()
            : base("name=ModelContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<SESENT_USERS> SESENT_USERS { get; set; }
        public DbSet<SESENT_Order> SESENT_Order { get; set; }
        public DbSet<SESENT_PhoneXCodes> SESENT_PhoneXCodes { get; set; }
        public DbSet<SESENT_Channels> SESENT_Channels { get; set; }
        public DbSet<SESENT_MessageText> SESENT_MessageText { get; set; }
        public DbSet<SESENT_MessageObj> SESENT_MessageObj { get; set; }
        public DbSet<SESENT_Settings> SESENT_Settings { get; set; }
        public DbSet<SESENT_ChannelProtocol> SESENT_ChannelProtocol { get; set; }
        public DbSet<SESENT_AdminManger> SESENT_AdminManger { get; set; }
        public DbSet<SESENT_ConsumptERecords> SESENT_ConsumptERecords { get; set; }
        public DbSet<SESENT_RankingSystemSetting> SESENT_RankingSystemSetting { get; set; }
        public DbSet<SESENT_CashCard> SESENT_CashCard { get; set; }
        public DbSet<SESENT_ChannelQuoTa> SESENT_ChannelQuoTa { get; set; }
        public DbSet<SESENT_BankLineNumber> SESENT_BankLineNumber { get; set; }
        public DbSet<SESENT_BankCity> SESENT_BankCity { get; set; }
    }
}
