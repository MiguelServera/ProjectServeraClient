# ProjectServeraClient
DATABASE SCRIPTS

CREATE TABLE [dbo].[Players] (
    [Id]         NVARCHAR (128) NOT NULL,
    [Name]       NVARCHAR (256) NULL,
    [Nickname]   NVARCHAR (256) NULL,
    [Email]      NVARCHAR (256) NULL,
    [Country]    NVARCHAR (256) NULL,
    [BirthDay]   DATETIME2 (7)  NULL,
    [DateJoined] DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    [Level]      INT            DEFAULT ((1)) NOT NULL,
    [BlobUri]    NVARCHAR (512) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Players_To_AspNetUsers] FOREIGN KEY ([Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

CREATE TABLE [dbo].[Games] (
    [Id]           NVARCHAR (256) NOT NULL,
    [Nickname]     NVARCHAR (256) NOT NULL,
    [Score]        INT            NOT NULL,
    [Difficulty]   INT            NOT NULL,
    [DateStarted]  DATETIME2 (7)  NOT NULL,
    [DateFinished] DATETIME2 (7)  NOT NULL
);

CREATE TABLE [dbo].[PlayersOnline] (
    [Id]       NVARCHAR (256) NOT NULL,
    [Nickname] NVARCHAR (256) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

