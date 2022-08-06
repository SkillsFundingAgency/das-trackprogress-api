CREATE TABLE [Progress]
(
    [Id] bigint NOT NULL IDENTITY,
    [Ukprn] bigint NOT NULL,
    [Uln] bigint NOT NULL,
    [StartDate] date NOT NULL,
    [ApprovalId] bigint NOT NULL,
    [OnTrack] bit NOT NULL,
	[CreatedOn] datetime2 NOT NULL DEFAULT current_timestamp, 
    CONSTRAINT [PK_Progress] PRIMARY KEY ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_Progress_UKPRN_ULN_StartDate] ON [Progress]
(
	[Ukprn] ASC,
	[Uln] ASC,
	[StartDate] ASC
)