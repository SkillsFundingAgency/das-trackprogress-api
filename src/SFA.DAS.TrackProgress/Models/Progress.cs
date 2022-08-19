namespace SFA.DAS.TrackProgress.Models;

public class Progress
{
    private Progress()
    {
        Apprenticeship = null!;
        Approval = null!;
        ProgressData = null!;
    }

    public Progress(ApprenticeshipId apprenticeship, ApprovalId approval, KsbTaxonomy ksbs)
    {
        Apprenticeship = apprenticeship;
        Approval = approval;
        ProgressData = ksbs;
    }

    public long Id { get; private set; }
    public ApprenticeshipId Apprenticeship { get; private set; }
    public ApprovalId Approval { get; private set; }
    public long ProgressDataVersion { get; private set; } = 1;
    public KsbTaxonomy ProgressData { get; private set; }
}