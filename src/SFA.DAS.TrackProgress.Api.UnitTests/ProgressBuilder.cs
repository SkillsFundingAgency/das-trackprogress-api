using Bogus;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public static class Some
{
    private static readonly Faker _faker = new("en");
    private static DateOnly MostRecentSubmission = _faker.Date.PastDateOnly(yearsToGoBack: 10);

    public static ProgressBuilder Progress
    {
        get
        {
            // Ensure that multiple Some.Progress are created in chronological order by default
            MostRecentSubmission = _faker.Date.SoonDateOnly(days: 7, refDate: MostRecentSubmission.AddDays(1));
            return new ProgressBuilder().SubmittedOn(MostRecentSubmission);
        }
    }
}

public record ProgressBuilder
{
    private static readonly Faker _faker = new("en");

    public long ProviderId { get; private init; } = _faker.Random.Number(100, 199);
    public long CommitmentsApprenticeshipId { get; private init; } = _faker.Random.Number(200, 299);
    public (string Id, int Value)[] Ksbs { get; private set; }
        = new[] { (_faker.Random.Guid().ToString(), _faker.Random.Int()) };
    public DateOnly CreatedOn { get; private set; } = _faker.Date.PastDateOnly();
    public string StandardUid { get; private set; } = _faker.Name.JobTitle();

    internal ProgressBuilder ForApprenticeship(long caid)
        => new ProgressBuilder(this) with { CommitmentsApprenticeshipId = caid };

    internal ProgressBuilder FromProvider(long providerId)
        => new ProgressBuilder(this) with { ProviderId = providerId };

    public ProgressBuilder WithKsbs(params (string Id, int Value)[] ksbs)
        => new ProgressBuilder(this) with { Ksbs = ksbs };

    internal ProgressBuilder SubmittedOn(DateTime date)
        => SubmittedOn(DateOnly.FromDateTime(date));

    internal ProgressBuilder SubmittedOn(DateOnly date)
        => new ProgressBuilder(this) with { CreatedOn = date };

    internal ProgressBuilder OnStandard(string standardUid)
        => new ProgressBuilder(this) with { StandardUid = standardUid };

    public static implicit operator Progress(ProgressBuilder builder)
    {
        return Progress.CreateWithDate(
            new ProviderApprenticeshipIdentifier(builder.ProviderId, 1, DateTime.MinValue),
            new ApprovalId(builder.CommitmentsApprenticeshipId, null),
            builder.StandardUid,
            new KsbTaxonomy(builder.Ksbs.Select(ksb => new KsbTaxonomyItem(ksb.Id, ksb.Value)).ToArray()),
            builder.CreatedOn);
    }
}