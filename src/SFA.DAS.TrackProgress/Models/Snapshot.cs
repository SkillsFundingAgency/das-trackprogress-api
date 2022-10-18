namespace SFA.DAS.TrackProgress.Models;

public class Snapshot
{
    private Snapshot()
    {
        Approval = null!;
        Details = null!;
    }

    public Snapshot(ApprovalId approval, List<SnapshotDetail> details)
    {
        Approval = approval;
        Details = details;
    }

    public long Id { get; private set; }
    
    public ApprovalId Approval { get; private set; }
    
    public List<SnapshotDetail> Details { get; private set; }
}

public class SnapshotDetail
{
    public SnapshotDetail(string ksbId, int progressValue)
    {
        KsbId = ksbId;
        ProgressValue = progressValue;
    }

    public string KsbId { get; private set; }

    public int ProgressValue { get; private set; }
}