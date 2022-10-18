using Bogus;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public static class Some
{
    public static ProgressBuilder Progress => new();
}

public record ProgressBuilder
{
    private static readonly Faker _faker = new("en");
    public long ProviderId { get; private init; } = _faker.Random.Number(100, 199);
    public long CommitmentsApprenticeshipId { get; private init; } = _faker.Random.Number(200, 299);
    public (string Id, int Value)[] Ksbs { get; private set; }
        = new[] { (_faker.Random.Guid().ToString(), _faker.Random.Int()) };
    public DateOnly CreatedOn { get; private set; } = _faker.Date.PastDateOnly();

    internal ProgressBuilder ForApprenticeship(long caid)
        => new ProgressBuilder(this) with { CommitmentsApprenticeshipId = caid };

    internal ProgressBuilder FromProvider(long providerId)
        => new ProgressBuilder(this) with { ProviderId = providerId };

    public ProgressBuilder WithKsbs(params (string Id, int Value)[] ksbs)
        => new ProgressBuilder(this) with { Ksbs = ksbs };

    internal ProgressBuilder SubmittedOn(DateTime date)
        => new ProgressBuilder(this) with { CreatedOn = DateOnly.FromDateTime(date) };

    public static implicit operator Progress(ProgressBuilder builder)
    {
        return Progress.CreateWithDate(
            new ProviderApprenticeshipIdentifier(builder.ProviderId, 1, DateTime.MinValue),
            new ApprovalId(builder.CommitmentsApprenticeshipId, null),
            new KsbTaxonomy(builder.Ksbs.Select(ksb => new KsbTaxonomyItem(ksb.Id, ksb.Value)).ToArray()),
            builder.CreatedOn);
    }
}