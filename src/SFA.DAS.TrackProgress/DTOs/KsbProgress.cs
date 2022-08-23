using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.DTOs;

public class ProgressDto
{
    public ProviderApprenticeshipIdentifier ProviderApprenticeshipIdentifier { get; set; } = null!;
    public long? ApprenticeshipContinuationId { get; set; }
    public ProgressItem[] Ksbs { get; set; } = null!;
}

