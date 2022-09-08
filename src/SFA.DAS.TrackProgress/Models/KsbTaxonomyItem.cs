namespace SFA.DAS.TrackProgress.Models;

public class KsbTaxonomyItem
{
    private KsbTaxonomyItem()
    {
        Id = "";
    }

    public KsbTaxonomyItem(string id, int value) => (Id, Value) = (id, value);

    public string Id { get; private set; }
    public int Value { get; private set; }
}