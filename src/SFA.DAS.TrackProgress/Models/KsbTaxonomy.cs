namespace SFA.DAS.TrackProgress.Models;

public class KsbTaxonomy
{
    public KsbTaxonomy(KsbTaxonomyItem[] ksbs)
        => Ksbs = ksbs;

    public KsbTaxonomyItem[] Ksbs { get; private set; }
}