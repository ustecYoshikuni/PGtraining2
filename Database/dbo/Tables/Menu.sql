CREATE TABLE [dbo].[Menu] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [OrderNo]  NCHAR (8)     NOT NULL,
    [MenuCode] VARCHAR (8)   NOT NULL,
    [MenuName] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__tmp_ms_x__3214EC0753AF0251] PRIMARY KEY CLUSTERED ([Id] ASC)
);

