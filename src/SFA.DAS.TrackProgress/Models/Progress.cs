namespace SFA.DAS.TrackProgress.Models;

public class Progress
{
    private Progress()
    {
        ProviderApprenticeshipIdentifier = null!;
        Approval = null!;
        ProgressData = null!;
    }

    public Progress(ProviderApprenticeshipIdentifier apprenticeship, ApprovalId approval, KsbTaxonomy ksbs)
    {
        ProviderApprenticeshipIdentifier = apprenticeship;
        Approval = approval;
        ProgressData = ksbs;
    }

    public long Id { get; private set; }
    public ProviderApprenticeshipIdentifier ProviderApprenticeshipIdentifier { get; private set; }
    public ApprovalId Approval { get; private set; }
    public long ProgressDataVersion { get; private set; } = 1;
    public KsbTaxonomy ProgressData { get; private set; }
}