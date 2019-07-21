
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/21/2019 23:44:23
-- Generated from EDMX file: E:\Xlettlery\Model\Model\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Xlettery];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[SESENT_USERS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_USERS];
GO
IF OBJECT_ID(N'[dbo].[SESENT_Order]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_Order];
GO
IF OBJECT_ID(N'[dbo].[SESENT_PhoneXCodes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_PhoneXCodes];
GO
IF OBJECT_ID(N'[dbo].[SESENT_Channels]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_Channels];
GO
IF OBJECT_ID(N'[dbo].[SESENT_MessageText]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_MessageText];
GO
IF OBJECT_ID(N'[dbo].[SESENT_MessageObj]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_MessageObj];
GO
IF OBJECT_ID(N'[dbo].[SESENT_Settings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_Settings];
GO
IF OBJECT_ID(N'[dbo].[SESENT_ChannelProtocol]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_ChannelProtocol];
GO
IF OBJECT_ID(N'[dbo].[SESENT_AdminManger]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_AdminManger];
GO
IF OBJECT_ID(N'[dbo].[SESENT_ConsumptERecords]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_ConsumptERecords];
GO
IF OBJECT_ID(N'[dbo].[SESENT_RankingSystemSetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_RankingSystemSetting];
GO
IF OBJECT_ID(N'[dbo].[SESENT_CashCard]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_CashCard];
GO
IF OBJECT_ID(N'[dbo].[SESENT_ChannelQuoTa]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_ChannelQuoTa];
GO
IF OBJECT_ID(N'[dbo].[SESENT_BankLineNumber]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_BankLineNumber];
GO
IF OBJECT_ID(N'[dbo].[SESENT_BankCity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_BankCity];
GO
IF OBJECT_ID(N'[dbo].[SESENT_Lottery]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_Lottery];
GO
IF OBJECT_ID(N'[dbo].[SESENT_FootBallMatch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_FootBallMatch];
GO
IF OBJECT_ID(N'[dbo].[SESENT_FootBallGame]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_FootBallGame];
GO
IF OBJECT_ID(N'[dbo].[SESENT_BasketBallMatch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_BasketBallMatch];
GO
IF OBJECT_ID(N'[dbo].[SESENT_BasketBallGame]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_BasketBallGame];
GO
IF OBJECT_ID(N'[dbo].[SESENT_KJLottery]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_KJLottery];
GO
IF OBJECT_ID(N'[dbo].[SESENT_InfoMation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_InfoMation];
GO
IF OBJECT_ID(N'[dbo].[SESENT_FootBallOrder]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_FootBallOrder];
GO
IF OBJECT_ID(N'[dbo].[SESENT_GenDan]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SESENT_GenDan];
GO
IF OBJECT_ID(N'[dbo].[SENENT_GuanZhu]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SENENT_GuanZhu];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'SESENT_USERS'
CREATE TABLE [dbo].[SESENT_USERS] (
    [userName] nvarchar(50)  NOT NULL,
    [userPwd] nvarchar(300)  NOT NULL,
    [Phone] nvarchar(11)  NOT NULL,
    [UseAmount] decimal(18,0)  NOT NULL,
    [userType] smallint  NOT NULL,
    [userPayPwd] nvarchar(max)  NOT NULL,
    [SuperiorAgent] nvarchar(max)  NULL,
    [AgentMoney] decimal(18,0)  NOT NULL,
    [AccountID] nvarchar(max)  NOT NULL,
    [ID] int IDENTITY(1,1) NOT NULL,
    [Lv] smallint  NOT NULL,
    [Recharge] decimal(18,0)  NOT NULL,
    [Consumption] decimal(18,0)  NOT NULL,
    [UpgradeTime] datetime  NULL,
    [RealName] nvarchar(20)  NULL,
    [IDCardNum] nvarchar(20)  NOT NULL,
    [HeadImg] varbinary(max)  NULL
);
GO

-- Creating table 'SESENT_Order'
CREATE TABLE [dbo].[SESENT_Order] (
    [OrderID] nvarchar(max)  NOT NULL,
    [OrderType] smallint  NOT NULL,
    [AccountID] nvarchar(max)  NOT NULL,
    [InputMoney] decimal(18,0)  NOT NULL,
    [OutMoney] decimal(18,0)  NOT NULL,
    [Status] smallint  NOT NULL,
    [ID] int IDENTITY(1,1) NOT NULL,
    [OrderTime] datetime  NOT NULL,
    [ChannelID] nvarchar(50)  NOT NULL,
    [BankID] int  NULL,
    [FailReason] nvarchar(max)  NULL
);
GO

-- Creating table 'SESENT_PhoneXCodes'
CREATE TABLE [dbo].[SESENT_PhoneXCodes] (
    [Life] smallint  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [Type] smallint  NOT NULL,
    [ID] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'SESENT_Channels'
CREATE TABLE [dbo].[SESENT_Channels] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ChannelID] nvarchar(100)  NOT NULL,
    [ChannelName] nvarchar(50)  NOT NULL,
    [Status] smallint  NOT NULL,
    [ChannelType] smallint  NOT NULL,
    [ProtocolID] nvarchar(50)  NOT NULL,
    [QuotaType] smallint  NOT NULL
);
GO

-- Creating table 'SESENT_MessageText'
CREATE TABLE [dbo].[SESENT_MessageText] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SendID] nvarchar(max)  NOT NULL,
    [CreateDateTime] datetime  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Type] smallint  NOT NULL,
    [Recld] nvarchar(max)  NOT NULL,
    [Gift] decimal(18,0)  NOT NULL,
    [HasGift] bit  NOT NULL
);
GO

-- Creating table 'SESENT_MessageObj'
CREATE TABLE [dbo].[SESENT_MessageObj] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RecId] nvarchar(max)  NOT NULL,
    [ReciveDateTime] datetime  NOT NULL,
    [Status] smallint  NOT NULL,
    [MessageID] int  NOT NULL
);
GO

-- Creating table 'SESENT_Settings'
CREATE TABLE [dbo].[SESENT_Settings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [image] nvarchar(max)  NOT NULL,
    [Status] smallint  NOT NULL,
    [Type] smallint  NOT NULL,
    [Width] smallint  NOT NULL,
    [Height] smallint  NOT NULL
);
GO

-- Creating table 'SESENT_ChannelProtocol'
CREATE TABLE [dbo].[SESENT_ChannelProtocol] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProtocolID] nvarchar(50)  NOT NULL,
    [ProtocolName] nvarchar(100)  NOT NULL,
    [ConfigFile] varbinary(max)  NOT NULL,
    [Status] smallint  NOT NULL,
    [EntriTime] datetime  NOT NULL
);
GO

-- Creating table 'SESENT_AdminManger'
CREATE TABLE [dbo].[SESENT_AdminManger] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [userName] nvarchar(100)  NOT NULL,
    [userPwd] nvarchar(100)  NOT NULL,
    [LoginLastTime] datetime  NOT NULL,
    [IP] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'SESENT_ConsumptERecords'
CREATE TABLE [dbo].[SESENT_ConsumptERecords] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AccountID] nvarchar(50)  NOT NULL,
    [Withdrawable] decimal(18,0)  NOT NULL,
    [OrderID] nvarchar(50)  NOT NULL,
    [EntryTime] datetime  NOT NULL,
    [Status] smallint  NOT NULL,
    [OutMoney] decimal(18,0)  NOT NULL
);
GO

-- Creating table 'SESENT_RankingSystemSetting'
CREATE TABLE [dbo].[SESENT_RankingSystemSetting] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Lv] smallint  NOT NULL,
    [Recharge] decimal(18,0)  NOT NULL,
    [Consumption] decimal(18,0)  NOT NULL,
    [Reward] smallint  NOT NULL,
    [UpgradeAward] decimal(18,0)  NOT NULL,
    [InsuredAmount] decimal(18,0)  NOT NULL
);
GO

-- Creating table 'SESENT_CashCard'
CREATE TABLE [dbo].[SESENT_CashCard] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AccountID] nvarchar(max)  NOT NULL,
    [BankName] nvarchar(max)  NOT NULL,
    [BankCode] nvarchar(max)  NOT NULL,
    [LineNumber] nvarchar(max)  NOT NULL,
    [NetSite] nvarchar(max)  NOT NULL,
    [BankNumber] nvarchar(max)  NOT NULL,
    [EnterTime] datetime  NOT NULL
);
GO

-- Creating table 'SESENT_ChannelQuoTa'
CREATE TABLE [dbo].[SESENT_ChannelQuoTa] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ChannelID] nvarchar(max)  NOT NULL,
    [Fee] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SESENT_BankLineNumber'
CREATE TABLE [dbo].[SESENT_BankLineNumber] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [BankCode] nvarchar(max)  NOT NULL,
    [BankName] nvarchar(max)  NOT NULL,
    [NetSite] nvarchar(max)  NOT NULL,
    [LineNumber] nvarchar(max)  NOT NULL,
    [Provinceid] int  NOT NULL
);
GO

-- Creating table 'SESENT_BankCity'
CREATE TABLE [dbo].[SESENT_BankCity] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Province] nvarchar(max)  NOT NULL,
    [City] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SESENT_Lottery'
CREATE TABLE [dbo].[SESENT_Lottery] (
    [lotteryId] int IDENTITY(1,1) NOT NULL,
    [lottery_name] nvarchar(20)  NOT NULL,
    [remarks] nvarchar(50)  NOT NULL,
    [Status] smallint  NOT NULL,
    [Type] int  NOT NULL
);
GO

-- Creating table 'SESENT_FootBallMatch'
CREATE TABLE [dbo].[SESENT_FootBallMatch] (
    [FootballID] bigint  NOT NULL,
    [No] nvarchar(20)  NOT NULL,
    [Match] nvarchar(10)  NOT NULL,
    [EndTime] nvarchar(50)  NOT NULL,
    [MainteamRanking] nvarchar(20)  NOT NULL,
    [Mainteam] nvarchar(50)  NOT NULL,
    [Visitingteam] nvarchar(50)  NOT NULL,
    [VisitingteamRanking] nvarchar(10)  NOT NULL,
    [MatchDate] datetime  NOT NULL,
    [MatchWeek] nvarchar(10)  NOT NULL,
    [Fk_FnID] nvarchar(200)  NOT NULL,
    [BeginTime] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SESENT_FootBallGame'
CREATE TABLE [dbo].[SESENT_FootBallGame] (
    [FId] bigint IDENTITY(1,1) NOT NULL,
    [FootballID] bigint  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Source] nvarchar(max)  NOT NULL,
    [Type] int  NOT NULL,
    [Code] nvarchar(200)  NOT NULL,
    [Lable] nvarchar(max)  NULL
);
GO

-- Creating table 'SESENT_BasketBallMatch'
CREATE TABLE [dbo].[SESENT_BasketBallMatch] (
    [BasketballID] bigint  NOT NULL,
    [No] nvarchar(20)  NOT NULL,
    [Match] nvarchar(10)  NOT NULL,
    [EndTime] nvarchar(50)  NOT NULL,
    [MainteamRanking] nvarchar(20)  NOT NULL,
    [Mainteam] nvarchar(50)  NOT NULL,
    [Visitingteam] nvarchar(50)  NOT NULL,
    [VisitingteamRanking] nvarchar(10)  NOT NULL,
    [MatchDate] datetime  NOT NULL,
    [MatchWeek] nvarchar(10)  NOT NULL,
    [Fk_FnID] nvarchar(200)  NOT NULL,
    [BeginTime] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SESENT_BasketBallGame'
CREATE TABLE [dbo].[SESENT_BasketBallGame] (
    [Fid] bigint IDENTITY(1,1) NOT NULL,
    [BasketballID] bigint  NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Source] nvarchar(max)  NOT NULL,
    [Type] int  NOT NULL,
    [Code] nvarchar(200)  NOT NULL,
    [Lable] nvarchar(max)  NULL
);
GO

-- Creating table 'SESENT_KJLottery'
CREATE TABLE [dbo].[SESENT_KJLottery] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LotteryMoney] decimal(18,2)  NOT NULL,
    [No] nvarchar(50)  NOT NULL,
    [Type] int  NOT NULL,
    [Source] nvarchar(100)  NOT NULL,
    [LotteryTime] datetime  NOT NULL,
    [GameType] int  NOT NULL
);
GO

-- Creating table 'SESENT_InfoMation'
CREATE TABLE [dbo].[SESENT_InfoMation] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EnterTime] datetime  NOT NULL,
    [Information] nvarchar(max)  NOT NULL,
    [Keyword] nvarchar(10)  NOT NULL,
    [Title] nvarchar(200)  NOT NULL,
    [ReaderCount] int  NOT NULL,
    [Conver] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SESENT_FootBallOrder'
CREATE TABLE [dbo].[SESENT_FootBallOrder] (
    [OrderID] bigint IDENTITY(1,1) NOT NULL,
    [FIds] nvarchar(max)  NOT NULL,
    [Status] smallint  NOT NULL,
    [EnterTime] datetime  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [AccountID] nvarchar(100)  NOT NULL,
    [GameType] int  NOT NULL
);
GO

-- Creating table 'SESENT_GenDan'
CREATE TABLE [dbo].[SESENT_GenDan] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AccountID] nvarchar(max)  NOT NULL,
    [CreateDateTime] nvarchar(max)  NOT NULL,
    [GameType] nvarchar(max)  NOT NULL,
    [GameID] nvarchar(max)  NOT NULL,
    [EndTime] nvarchar(max)  NOT NULL,
    [Status] smallint  NULL,
    [BeginMoney] decimal(18,0)  NOT NULL
);
GO

-- Creating table 'SENENT_GuanZhu'
CREATE TABLE [dbo].[SENENT_GuanZhu] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [followAccountID] nvarchar(max)  NOT NULL,
    [AccountID] nvarchar(max)  NOT NULL,
    [Status] smallint  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'SESENT_USERS'
ALTER TABLE [dbo].[SESENT_USERS]
ADD CONSTRAINT [PK_SESENT_USERS]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SESENT_Order'
ALTER TABLE [dbo].[SESENT_Order]
ADD CONSTRAINT [PK_SESENT_Order]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'SESENT_PhoneXCodes'
ALTER TABLE [dbo].[SESENT_PhoneXCodes]
ADD CONSTRAINT [PK_SESENT_PhoneXCodes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_Channels'
ALTER TABLE [dbo].[SESENT_Channels]
ADD CONSTRAINT [PK_SESENT_Channels]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_MessageText'
ALTER TABLE [dbo].[SESENT_MessageText]
ADD CONSTRAINT [PK_SESENT_MessageText]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_MessageObj'
ALTER TABLE [dbo].[SESENT_MessageObj]
ADD CONSTRAINT [PK_SESENT_MessageObj]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_Settings'
ALTER TABLE [dbo].[SESENT_Settings]
ADD CONSTRAINT [PK_SESENT_Settings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_ChannelProtocol'
ALTER TABLE [dbo].[SESENT_ChannelProtocol]
ADD CONSTRAINT [PK_SESENT_ChannelProtocol]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_AdminManger'
ALTER TABLE [dbo].[SESENT_AdminManger]
ADD CONSTRAINT [PK_SESENT_AdminManger]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_ConsumptERecords'
ALTER TABLE [dbo].[SESENT_ConsumptERecords]
ADD CONSTRAINT [PK_SESENT_ConsumptERecords]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_RankingSystemSetting'
ALTER TABLE [dbo].[SESENT_RankingSystemSetting]
ADD CONSTRAINT [PK_SESENT_RankingSystemSetting]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_CashCard'
ALTER TABLE [dbo].[SESENT_CashCard]
ADD CONSTRAINT [PK_SESENT_CashCard]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_ChannelQuoTa'
ALTER TABLE [dbo].[SESENT_ChannelQuoTa]
ADD CONSTRAINT [PK_SESENT_ChannelQuoTa]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_BankLineNumber'
ALTER TABLE [dbo].[SESENT_BankLineNumber]
ADD CONSTRAINT [PK_SESENT_BankLineNumber]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'SESENT_BankCity'
ALTER TABLE [dbo].[SESENT_BankCity]
ADD CONSTRAINT [PK_SESENT_BankCity]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [lotteryId] in table 'SESENT_Lottery'
ALTER TABLE [dbo].[SESENT_Lottery]
ADD CONSTRAINT [PK_SESENT_Lottery]
    PRIMARY KEY CLUSTERED ([lotteryId] ASC);
GO

-- Creating primary key on [FootballID] in table 'SESENT_FootBallMatch'
ALTER TABLE [dbo].[SESENT_FootBallMatch]
ADD CONSTRAINT [PK_SESENT_FootBallMatch]
    PRIMARY KEY CLUSTERED ([FootballID] ASC);
GO

-- Creating primary key on [FId] in table 'SESENT_FootBallGame'
ALTER TABLE [dbo].[SESENT_FootBallGame]
ADD CONSTRAINT [PK_SESENT_FootBallGame]
    PRIMARY KEY CLUSTERED ([FId] ASC);
GO

-- Creating primary key on [BasketballID] in table 'SESENT_BasketBallMatch'
ALTER TABLE [dbo].[SESENT_BasketBallMatch]
ADD CONSTRAINT [PK_SESENT_BasketBallMatch]
    PRIMARY KEY CLUSTERED ([BasketballID] ASC);
GO

-- Creating primary key on [Fid] in table 'SESENT_BasketBallGame'
ALTER TABLE [dbo].[SESENT_BasketBallGame]
ADD CONSTRAINT [PK_SESENT_BasketBallGame]
    PRIMARY KEY CLUSTERED ([Fid] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_KJLottery'
ALTER TABLE [dbo].[SESENT_KJLottery]
ADD CONSTRAINT [PK_SESENT_KJLottery]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_InfoMation'
ALTER TABLE [dbo].[SESENT_InfoMation]
ADD CONSTRAINT [PK_SESENT_InfoMation]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [OrderID] in table 'SESENT_FootBallOrder'
ALTER TABLE [dbo].[SESENT_FootBallOrder]
ADD CONSTRAINT [PK_SESENT_FootBallOrder]
    PRIMARY KEY CLUSTERED ([OrderID] ASC);
GO

-- Creating primary key on [Id] in table 'SESENT_GenDan'
ALTER TABLE [dbo].[SESENT_GenDan]
ADD CONSTRAINT [PK_SESENT_GenDan]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SENENT_GuanZhu'
ALTER TABLE [dbo].[SENENT_GuanZhu]
ADD CONSTRAINT [PK_SENENT_GuanZhu]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------