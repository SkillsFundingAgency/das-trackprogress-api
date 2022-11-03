namespace SFA.DAS.TrackProgress.Models;

public class KsbName
{
    private KsbName()
    {
        Id = null!;
        Type = null!;
        Name = null!;
    }

    public KsbName(string id, string type, string name)
    {
        Id = id;
        Type = type;
        Name = name;
    }

    public string Id { get; private set; }
    public string Type { get; set; }
    public string Name { get; set; }
}