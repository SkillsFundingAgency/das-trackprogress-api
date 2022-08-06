using SFA.DAS.TrackProgress.DTOs;

namespace SFA.DAS.TrackProgress.Models;

public class Progress
{
    private Progress()
    { }

    public Progress(ApprenticeshipId apprenticeship, long approvalId, bool onTrack)
    {
        Apprenticeship = apprenticeship;
        ApprovalId = approvalId;
        OnTrack = onTrack;
    }

    public long Id { get; private set; }
    public ApprenticeshipId Apprenticeship { get; private set; } = null!;
    public long ApprovalId { get; private set; }
    public bool OnTrack { get; private set; }
}