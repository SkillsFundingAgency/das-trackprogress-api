namespace SFA.DAS.TrackProgress.Models;

public record ApprenticeshipId(long Ukprn, long Uln, DateOnly StartDate);

public record ApprovalId(long ApprenticeshipId, long? ContinuationId);