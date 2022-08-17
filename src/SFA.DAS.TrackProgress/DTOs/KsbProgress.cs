namespace SFA.DAS.TrackProgress.DTOs;

public class KsbProgress
{
    public long ApprovalId { get; set; }
    public long ApprovalContinuationId { get; set; }
    public long ApprenticeshipId { get; set; }
    public ProgressItem[] Knowledges { get; set; } = null!;
    public ProgressItem[] Skills { get; set; } = null!;
    public ProgressItem[] Behaviours { get; set; } = null!;
}