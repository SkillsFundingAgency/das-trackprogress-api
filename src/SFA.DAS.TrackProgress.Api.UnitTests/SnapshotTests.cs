﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NServiceBus.Testing;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class SnapshotTests : ApiFixture
{
    [Test]
    public void Some_progress_is_submitted_in_order_created_by_default()
    {
        var manyProgress = Enumerable.Range(1, 50).Select(x => Some.Progress);
        manyProgress.Should().BeInAscendingOrder(p => p.CreatedOn);
    }

    [Test]
    public async Task Save_progress_from_single_event()
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
        var response = await Client.PostAsync("/apprenticeships/1/snapshot", null);

        // Then
        response.Should().Be201Created();
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
    public async Task Save_progress_from_two_events_without_overlap()
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
        var response = await Client.PostAsync("/apprenticeships/1/snapshot", null);

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
    public async Task Save_progress_from_two_overlapping_events()
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
        var response = await Client.PostAsync("/apprenticeships/1/snapshot", null);

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
    public async Task Save_progress_from_two_overlapping_events_inverted_order()
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
        var response = await Client.PostAsync("/apprenticeships/1/snapshot", null);

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
    public async Task Save_progress_when_second_event_reduces_value()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs((Id: "123", Value: 80)));

            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs((Id: "123", Value: 70)));

            return db.SaveChangesAsync();
        });

        // When
        var response = await Client.PostAsync("/apprenticeships/1/snapshot", null);
        response.EnsureSuccessStatusCode();

        // Then
        await VerifyDatabase(db =>
        {
            db.Snapshot.Include(x => x.Details).Should().ContainEquivalentOf(new
            {
                Details = new[]
                {
                    new { KsbId = "123", ProgressValue = 70 },
                }
            });
        });
    }

    [Test]
    public async Task Save_progress_publishes_NewPublishedAddedEvent_when_KSBs_are_missing()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .OnStandard("Cinematography_1.1")
                .WithKsbs(
                    (Id: "12", Value: 99),
                    (Id: "15", Value: 55)));

            return db.SaveChangesAsync();
        });

        // When
        var response = await Client.PostAsync("/apprenticeships/1/snapshot", null);

        // Then
        Messages.SentMessages.Should().ContainEquivalentOf(new
        {
            Message = new
            {
                StandardUid = "Cinematography_1.1",
            }
        });
    }

    [Test]
    public async Task Save_progress_does_not_publish_NewPublishedAddedEvent_when_KSBs_are_all_present()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .OnStandard("Cinematography_1.1")
                .WithKsbs(
                    (Id: "12", Value: 99),
                    (Id: "15", Value: 55)));

            db.KsbCache.AddRange(
                new("12", "K", "99"),
                new("15", "K", "55"));

            return db.SaveChangesAsync();
        });

        // When
        var response = await Client.PostAsync("/apprenticeships/1/snapshot", null);

        // Then
        Messages.SentMessages.Should().BeEmpty();
    }

    [Test]
    public async Task Save_progress_publishes_NewPublishedAddedEvent_when_some_KSBs_are_present()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(12)
                .OnStandard("RockClimbing_4.9")
                .WithKsbs(
                    (Id: "12", Value: 99),
                    (Id: "15", Value: 55)));

            db.KsbCache.Add(new("15","B", "55"));

            return db.SaveChangesAsync();
        });

        // When
        var response = await Client.PostAsync("/apprenticeships/12/snapshot", null);

        // Then
        Messages.SentMessages.Should().ContainEquivalentOf(new
        {
            Message = new
            {
                StandardUid = "RockClimbing_4.9",
            }
        });
    }

    [Test]
    public async Task A_second_snapshot_is_created_should_delete_the_existing_snapshot()
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
        // We generate first snapshot
        await Client.PostAsync("/apprenticeships/1/snapshot", null);
        // and add a new progress record
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Some.Progress
                .ForApprenticeship(1)
                .WithKsbs(
                    (Id: "12", Value: 90),
                    (Id: "13", Value: 10))
            );

            return db.SaveChangesAsync();
        });

        // When 
        var response = await Client.PostAsync("/apprenticeships/1/snapshot", null);

        // Then
        response.Should().Be201Created();
        await VerifyDatabase(db =>
        {
            db.Snapshot.Include(x => x.Details).Should().ContainEquivalentOf(new
            {
                Approval = new { ApprenticeshipId = 1 },
                Details = new[]
                {
                    new { KsbId = "12", ProgressValue = 90 },
                    new { KsbId = "13", ProgressValue = 10 },
                    new { KsbId = "14", ProgressValue = 44 },
                },
            });
        });

        await VerifyDatabase(db =>
        {
            db.Snapshot.Count().Should().Be(1);
        });
    }
}