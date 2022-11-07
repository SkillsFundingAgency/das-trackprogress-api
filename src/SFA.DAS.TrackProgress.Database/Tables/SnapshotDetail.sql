create table [dbo].[SnapshotDetail]
(
    [Id] bigint not null primary key identity, 
    [SnapshotId] bigint not null, 
    [KsbId] uniqueidentifier not null, 
    [ProgressValue] smallint not null,
    constraint FK_Snapshot_SnapshotId foreign key ([SnapshotId]) references [Snapshot] ([Id]) ON DELETE CASCADE
)
