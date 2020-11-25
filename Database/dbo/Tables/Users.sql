CREATE TABLE [dbo].[Users] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [UserId]         VARCHAR (8)   NOT NULL,
    [Name]           NVARCHAR (32) NOT NULL,
    [PassWord]       NVARCHAR (32) NOT NULL,
    [ExpirationDate] DATETIME      CONSTRAINT [DF__Users__Expiratio__5AEE82B9] DEFAULT ('9999/12/31') NOT NULL,
    CONSTRAINT [PK__Users__1788CC4CF8FDEF19] PRIMARY KEY CLUSTERED ([UserId] ASC)
);

