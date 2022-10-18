using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class SnapshotTests : ApiFixture
{
    [Test]
    public async Task Save_progress_to_database_1()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs((Id: "12", Value: 88)));
            return db.SaveChangesAsync();
        });

        // When
        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

        // Then
        await VerifyDatabase(db =>
        {
            db.Snapshot.Should().ContainEquivalentOf(new
            {
                Approval = new { ApprenticeshipId = 1 },
            });
        });
    }

    [Test]
    public async Task Save_progres_from_single_event()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs(
                    (Id: "12", Value: 88),
                    (Id: "14", Value: 44))
                );
            return db.SaveChangesAsync();
        });

        // When
        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

        // Then
        await VerifyDatabase(db =>
        {
            db.Snapshot.Include(x => x.Details).Should().ContainEquivalentOf(new
            {
                Approval = new { ApprenticeshipId = 1 },
                Details = new[]
                {
                    new { KsbId = "12", ProgressValue = 88 },
                    new { KsbId = "14", ProgressValue = 44 },
                },
            });
        });
    }

    [Test]
    public async Task Save_progres_from_two_events_without_overlap()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs(
                    (Id: "12", Value: 88),
                    (Id: "14", Value: 44)));

            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs(
                    (Id: "13", Value: 33),
                    (Id: "15", Value: 55)));

            return db.SaveChangesAsync();
        });

        // When
        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

        // Then
        await VerifyDatabase(db =>
        {
            db.Snapshot.Include(x => x.Details).Should().ContainEquivalentOf(new
            {
                Approval = new { ApprenticeshipId = 1 },
                Details = new[]
                {
                    new { KsbId = "12", ProgressValue = 88 },
                    new { KsbId = "13", ProgressValue = 33 },
                    new { KsbId = "14", ProgressValue = 44 },
                    new { KsbId = "15", ProgressValue = 55 },
                },
            });
        });
    }

    [Test]
    public async Task Save_progres_from_two_overlapping_events()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs(
                    (Id: "12", Value: 88),
                    (Id: "14", Value: 44))
                .SubmittedOn(DateTime.Now.AddDays(-1)));

            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs(
                    (Id: "12", Value: 99),
                    (Id: "15", Value: 55))
                .SubmittedOn(DateTime.Now));

            return db.SaveChangesAsync();
        });

        // When
        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

        // Then
        await VerifyDatabase(db =>
        {
            db.Snapshot.Include(x => x.Details).Should().ContainEquivalentOf(new
            {
                Approval = new { ApprenticeshipId = 1 },
                Details = new[]
                {
                    new { KsbId = "12", ProgressValue = 99 },
                    new { KsbId = "14", ProgressValue = 44 },
                    new { KsbId = "15", ProgressValue = 55 },
                },
            });
        });
    }

    [Test]
    public async Task Save_progres_from_two_overlapping_events_inverted_order()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs(
                    (Id: "12", Value: 99),
                    (Id: "15", Value: 55))
                .SubmittedOn(DateTime.Now));

            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs(
                    (Id: "12", Value: 88),
                    (Id: "14", Value: 44))
                .SubmittedOn(DateTime.Now.AddDays(-1)));

            return db.SaveChangesAsync();
        });

        // When
        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

        // Then
        await VerifyDatabase(db =>
        {
            db.Snapshot.Include(x => x.Details).Should().ContainEquivalentOf(new
            {
                Approval = new { ApprenticeshipId = 1 },
                Details = new[]
                {
                    new { KsbId = "12", ProgressValue = 99 },
                    new { KsbId = "14", ProgressValue = 44 },
                    new { KsbId = "15", ProgressValue = 55 },
                },
            });
        });
    }
}
