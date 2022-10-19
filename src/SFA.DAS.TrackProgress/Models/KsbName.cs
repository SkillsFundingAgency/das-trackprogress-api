namespace SFA.DAS.TrackProgress.Models;

public class KsbName
{
    private KsbName()
    {
        Id = null!;
        Name = null!;
    }

    public KsbName(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; private set; }
    public string Name { get; private set; }
}