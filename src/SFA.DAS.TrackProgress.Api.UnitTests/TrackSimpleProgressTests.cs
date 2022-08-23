using System.Net.Http.Json;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.TrackProgress.Api.Tests;
using SFA.DAS.TrackProgress.DTOs;
using SFA.DAS.TrackProgress.Models;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class TrackSimpleProgressTests : ApiFixture
{
    [Test, AutoData]
    public async Task Save_progress_to_database(long apprenticeshipId)
    {
        var progress = new ProgressDto
        {
            ProviderApprenticeshipIdentifier = new ProviderApprenticeshipIdentifier(1099, 1234567, new DateTime(2022, 09, 01)),
            ApprenticeshipContinuationId = 1111,
            Ksbs = fixture.CreateMany<ProgressItem>().ToArray()
        };

        var response = await client.PostAsJsonAsync($"/apprenticeships/{apprenticeshipId}", progress);
        response.Should().Be200Ok();

        await VerifyDatabase(db =>
        {
            db.Progress.Should().ContainEquivalentOf(new
            {
                ProviderApprenticeshipIdentifier = new
                {
                    progress.ProviderApprenticeshipIdentifier.ProviderId,
                    progress.ProviderApprenticeshipIdentifier.Uln,
                    progress.ProviderApprenticeshipIdentifier.StartDate
                },
                Approval = new
                {
                    ApprenticeshipId = apprenticeshipId,
                    progress.ApprenticeshipContinuationId,
                },
                ProgressData = new
                {
                    progress.Ksbs,
                },
            });
        });
    }
}
