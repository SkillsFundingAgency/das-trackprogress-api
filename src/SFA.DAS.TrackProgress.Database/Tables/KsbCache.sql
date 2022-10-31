CREATE TABLE [dbo].[KsbCache]
(
    [Id] UNIQUEIDENTIFIER NOT null, 
    [Name]NVARCHAR(250) not null
    CONSTRAINT [PK_KsbCache] PRIMARY KEY ([Id])
)
