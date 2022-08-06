CREATE TABLE [dbo].[Progress]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Ukprn] BIGINT NOT NULL, 
    [Uln] BIT NOT NULL, 
    [OnTrack] BIT NOT NULL
)
