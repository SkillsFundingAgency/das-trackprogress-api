namespace SFA.DAS.TrackProgress.Models;

public record ProviderApprenticeshipIdentifier(long ProviderId, long Uln, DateTime StartDate);

public record ApprovalId(long ApprenticeshipId, long? ApprenticeshipContinuationId);