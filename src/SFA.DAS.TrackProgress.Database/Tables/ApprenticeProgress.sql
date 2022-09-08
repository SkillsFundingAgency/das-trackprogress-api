CREATE TABLE [Progress]
(
    [Id] bigint NOT NULL IDENTITY,
    [ProviderId] bigint NOT NULL,
    [Uln] bigint NOT NULL,
    [StartDate] DATETIME2 NOT NULL,
    [CommitmentsApprenticeshipId] bigint NOT NULL,
    [CommitmentsContinuationId] bigint NULL,
    [ProgressDataVersion] SMALLINT NOT NULL,
    [ProgressData] nvarchar(max) NOT NULL,
	[CreatedOn] datetime2 NOT NULL DEFAULT current_timestamp, 
    CONSTRAINT [PK_Progress] PRIMARY KEY ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Progress_ProviderId_ULN_StartDate] ON [Progress]
(
	[ProviderId] ASC,
	[Uln] ASC,
	[StartDate] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_Progress_ApprenticeshipId] ON [Progress]
(
	[CommitmentsApprenticeshipId] ASC
)