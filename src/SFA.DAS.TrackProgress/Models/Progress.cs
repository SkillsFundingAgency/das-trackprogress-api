using SFA.DAS.TrackProgress.DTOs;

namespace SFA.DAS.TrackProgress.Models;

public class Progress
{
    private Progress()
    { }

    public Progress(long ukprn, long uln, DateOnly startDate, ProgressDto progress)
    {
        Ukprn = ukprn;
        Uln = uln;
        OnTrack = progress.OnTrack ?? false;
    }

    public int Id { get; private set; }
    public long Ukprn { get; private set; }
    public long Uln { get; private set; }
    public bool OnTrack { get; private set; }
}