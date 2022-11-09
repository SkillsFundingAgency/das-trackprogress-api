create table [dbo].[Snapshot]
(
    [Id] bigint not null primary key identity, 
    [CommitmentsApprenticeshipId] bigint not null,
    [CommitmentsContinuationId] bigint null,
    [CreatedOn] datetime2 not null default current_timestamp, 
)
