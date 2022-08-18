using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.TrackProgress.DTOs;
using System.Net.Http.Json;

namespace SFA.DAS.TrackProgress.Api.Tests;

public class TrackSimpleProgress : ApiFixture
{
    [Test, AutoData]
    public async Task Save_progress_to_database(long ukprn, long uln, DateTime startDate, KsbProgress progress)
    {
        var response = await client.PostAsJsonAsync($"/apprenticeship/{ukprn}/{uln}/{startDate:O}/progress", progress);
        response.Should().Be200Ok();

        await VerifyDatabase(db =>
        {
            db.Progress.Should().ContainEquivalentOf(new
            {
                Apprenticeship = new
                {
                    Ukprn = ukprn,
                    Uln = uln,
                    StartDate = DateOnly.FromDateTime(startDate),
                },
                Approval = new
                {
                    ApprenticeshipId = progress.ApprovalId,
                    ContinuationId = progress.ApprovalContinuationId,
                },
                ProgressData = new
                {
                    progress.Ksbs,
                },
            });
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task Validate_ukprn(long ukprn)
    {
        var response = await client.PostAsJsonAsync($"/apprenticeship/{ukprn}/1/2022-08-01/progress", new KsbProgress());
        response.Should().BeAs(new
        {
            errors = new
            {
                ukprn = new[] { "UKPRN must be greater than zero." }
            }
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task Validate_uln(long uln)
    {
        var response = await client.PostAsJsonAsync($"/apprenticeship/1/{uln}/2022-08-01/progress", new KsbProgress());
        response.Should().BeAs(new
        {
            errors = new
            {
                uln = new[] { "ULN must be greater than zero." }
            }
        });
    }

    [TestCase("not-a-date")]
    public async Task Validate_uln(string startDate)
    {
        var response = await client.PostAsJsonAsync($"/apprenticeship/1/1/{startDate}/progress", new KsbProgress());
        response.Should().BeAs(new
        {
            errors = new
            {
                startDate = new[] { "The input was not valid." }
            }
        });
    }
}
