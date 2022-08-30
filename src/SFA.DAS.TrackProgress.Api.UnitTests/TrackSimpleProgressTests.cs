using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.TrackProgress.Api.Tests;
using SFA.DAS.TrackProgress.Application.Commands.RecordApprenticeshipProgress;
using SFA.DAS.TrackProgress.DTOs;
using System.Net.Http.Json;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class TrackSimpleProgressTests : ApiFixture
{
    [Test, AutoData]
    public async Task Save_progress_to_database(long apprenticeshipId)
    {
        var progress = new RecordApprenticeshipProgressCommand(
            ProviderId: 1099,
            Uln: 1234567,
            StartDate: new DateTime(2022, 09, 01),
            CommitmentsApprenticeshipId: apprenticeshipId,
            CommitmentsContinuationId: 1111,
            Ksbs: fixture.CreateMany<ProgressItem>().ToArray()
        );

        var response = await client.PostAsJsonAsync($"/progress", progress);
        response.Should().Be201Created();

        await VerifyDatabase(db =>
        {
            db.Progress.Should().ContainEquivalentOf(new
            {
                ProviderApprenticeshipIdentifier = new
                {
                    progress.ProviderId,
                    progress.Uln,
                    progress.StartDate,
                },
                Approval = new
                {
                    ApprenticeshipId = apprenticeshipId,
                    ApprenticeshipContinuationId = progress.CommitmentsContinuationId,
                },
                ProgressData = new
                {
                    progress.Ksbs,
                },
            });
        });
    }
}
