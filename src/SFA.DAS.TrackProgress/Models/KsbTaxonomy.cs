namespace SFA.DAS.TrackProgress.Models;

public class KsbTaxonomy
{
    public KsbTaxonomy(
        KsbTaxonomyItem[] knowledges,
        KsbTaxonomyItem[] skills,
        KsbTaxonomyItem[] behaviours)
        => (Knowledges, Skills, Behaviours) = (knowledges, skills, behaviours);

    public KsbTaxonomyItem[] Knowledges { get; private set; }
    public KsbTaxonomyItem[] Skills { get; private set; }
    public KsbTaxonomyItem[] Behaviours { get; private set; }
}
