namespace SFA.DAS.TrackProgress.Models;

public class Snapshot
{
    private Snapshot()
    {
        Approval = null!;
    }

    public Snapshot(ApprovalId approval) => Approval = approval;

    public long Id { get; private set; }
    public ApprovalId Approval { get; private set; }
}