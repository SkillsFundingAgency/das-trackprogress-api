using SFA.DAS.TrackProgress.DTOs;

namespace SFA.DAS.TrackProgress.Models;

public class Progress
{
    private Progress()
    { }

    public Progress(ApprenticeshipId apprenticeship, ProgressDto progress)
    {
        Apprenticeship = apprenticeship;
        OnTrack = progress.OnTrack ?? false;
    }

    public long Id { get; private set; }
    public ApprenticeshipId Apprenticeship { get; private set; } = null!;
    public bool OnTrack { get; private set; }
}