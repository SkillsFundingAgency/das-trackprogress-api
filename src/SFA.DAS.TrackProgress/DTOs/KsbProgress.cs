namespace SFA.DAS.TrackProgress.DTOs;

public class KsbProgress
{
    public long ApprovalId { get; set; }
    public long? ApprovalContinuationId { get; set; }
    public ProgressItem[] Ksbs { get; set; } = null!;
}

public class ProgressDto
{
    public long ProviderId { get; set; }
    public long Uln { get; set; }
    public long? ApprenticeshipContinuationId { get; set; }
    public DateOnly? StartDate { get; set; }
    public ProgressItem[] Ksbs { get; set; } = null!;
}