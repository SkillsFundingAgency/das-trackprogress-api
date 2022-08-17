namespace SFA.DAS.TrackProgress.Models;

public class Progress
{
    private Progress()
    {
        Apprenticeship = null!;
        ProgressData = null!;
    }

    public Progress(ApprenticeshipId apprenticeship, long approvalId, KsbTaxonomy knowledges)
    {
        Apprenticeship = apprenticeship;
        ApprovalId = approvalId;
        ProgressData = knowledges;
    }

    public long Id { get; private set; }
    public ApprenticeshipId Apprenticeship { get; private set; }
    public long ApprovalId { get; private set; }
    public long? ApprovalContinuationId { get; private set; }
    public KsbTaxonomy ProgressData { get; private set; }
}