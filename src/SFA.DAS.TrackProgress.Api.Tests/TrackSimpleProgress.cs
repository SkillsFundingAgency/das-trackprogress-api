using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.TrackProgress.DTOs;
using System.Net.Http.Json;

namespace SFA.DAS.TrackProgress.Api.Tests;

public class TrackSimpleProgress : ApiFixture
{
    [Test, AutoData]
    public async Task Save_progress_to_database(long ukprn, long uln, DateTime startDate, ProgressDto progress)
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
                OnTrack = progress.OnTrack.Value,
            });
        });
    }
}
