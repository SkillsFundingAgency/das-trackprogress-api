namespace SFA.DAS.TrackProgress.Models;

public class Progress
{
    public int Id { get; private set; }
    public long Ukprn { get; private set; }
    public long Uln { get; private set; }
    public bool OnTrack { get; private set; }
}