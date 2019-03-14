
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/14/2019 14:28:59
-- Generated from EDMX file: E:\Xlettlery\Xlettlery\Model\Model\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
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
    [SuperiorAgent] nvarchar(max)  NOT NULL,
    [AgentMoney] decimal(18,0)  NOT NULL,
    [AccountID] nvarchar(max)  NOT NULL,
    [ID] int IDENTITY(1,1) NOT NULL,
    [Lv] smallint  NOT NULL,
    [Recharge] decimal(18,0)  NOT NULL,
    [Consumption] decimal(18,0)  NOT NULL
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
    [ChannelID] nvarchar(50)  NOT NULL
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
    [LimitMin] int  NOT NULL,
    [LimitMax] int  NOT NULL,
    [ChannelType] smallint  NOT NULL,
    [ProtocolID] nvarchar(50)  NOT NULL
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

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------