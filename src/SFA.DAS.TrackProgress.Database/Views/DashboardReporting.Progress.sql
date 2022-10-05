CREATE VIEW [DashboardReporting].[Progress]
	AS 
SELECT 
	[Id],
    [ProviderId],
    [StartDate],
    [CommitmentsApprenticeshipId],
    [CommitmentsContinuationId],
    [ProgressDataVersion],
	[CreatedOn] 	
FROM dbo.Progress
