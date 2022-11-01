CREATE TABLE [dbo].[KsbCache]
(
    [Id] UNIQUEIDENTIFIER NOT null, 
    [Name]NVARCHAR(MAX) not null
    CONSTRAINT [PK_KsbCache] PRIMARY KEY ([Id])
)
