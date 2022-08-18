namespace SFA.DAS.TrackProgress.DTOs;

public class KsbProgress
{
    public long ApprovalId { get; set; }
    public long ApprovalContinuationId { get; set; }
    public long ApprenticeshipId { get; set; }
    public ProgressItem[] Ksbs { get; set; } = null!;
}