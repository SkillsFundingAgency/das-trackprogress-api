namespace SFA.DAS.TrackProgress.Models;

public class Progress
{
    private Progress()
    {
        ProviderApprenticeshipIdentifier = null!;
        Approval = null!;
        ProgressData = null!;
        StandardUid = null!;
    }

    public Progress(ProviderApprenticeshipIdentifier apprenticeship, ApprovalId approval, string standardUid, KsbTaxonomy ksbs)
    {
        ProviderApprenticeshipIdentifier = apprenticeship;
        Approval = approval;
        StandardUid = standardUid;
        ProgressData = ksbs;
    }

    public static Progress CreateWithDate(ProviderApprenticeshipIdentifier apprenticeship, ApprovalId approval, string standardUid, KsbTaxonomy ksbs, DateOnly createdOn)
    {
        return new(apprenticeship, approval, standardUid, ksbs)
        {
            CreatedOn = createdOn
        };
    }

    public long Id { get; private set; }
    public ProviderApprenticeshipIdentifier ProviderApprenticeshipIdentifier { get; private set; }
    public ApprovalId Approval { get; private set; }
    public string StandardUid { get; private set; }
    public short ProgressDataVersion { get; private set; } = 1;
    public KsbTaxonomy ProgressData { get; private set; }
    public DateOnly CreatedOn { get; private set;  }
}