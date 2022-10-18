using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class SnapshotTests : ApiFixture
{
    [Test]
    public async Task Save_progress_to_database_1()
    {
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(new Models.Progress(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.MinValue),
                new Models.ApprovalId(1, null),
                new Models.KsbTaxonomy(new[] { new Models.KsbTaxonomyItem("12", 88) })));
            return db.SaveChangesAsync();
        });

        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

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
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(new Models.Progress(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.MinValue),
                new Models.ApprovalId(1, null),
                new Models.KsbTaxonomy(new[]
                {
                    new Models.KsbTaxonomyItem("12", 88),
                    new Models.KsbTaxonomyItem("14", 44),
                })));
            return db.SaveChangesAsync();
        });

        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

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
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(new Models.Progress(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.MinValue),
                new Models.ApprovalId(1, null),
                new Models.KsbTaxonomy(new[]
                {
                    new Models.KsbTaxonomyItem("12", 88),
                    new Models.KsbTaxonomyItem("14", 44),
                })));
            db.Progress.Add(new Models.Progress(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.MinValue),
                new Models.ApprovalId(1, null),
                new Models.KsbTaxonomy(new[]
                {
                    new Models.KsbTaxonomyItem("13", 33),
                    new Models.KsbTaxonomyItem("15", 55),
                })));
            return db.SaveChangesAsync();
        });

        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

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
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Models.Progress.CreateWithDate(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.MinValue),
                new Models.ApprovalId(1, null),
                new Models.KsbTaxonomy(new[]
                {
                    new Models.KsbTaxonomyItem("12", 88),
                    new Models.KsbTaxonomyItem("14", 44),
                }),
                DateOnly.FromDateTime(DateTime.Now.AddDays(-1))));
            
            db.Progress.Add(Models.Progress.CreateWithDate(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.MinValue),
                new Models.ApprovalId(1, null),
                new Models.KsbTaxonomy(new[]
                {
                    new Models.KsbTaxonomyItem("12", 99),
                    new Models.KsbTaxonomyItem("15", 55),
                }),
                DateOnly.FromDateTime(DateTime.Now)));

            return db.SaveChangesAsync();
        });

        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

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
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(Models.Progress.CreateWithDate(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.MinValue),
                new Models.ApprovalId(1, null),
                new Models.KsbTaxonomy(new[]
                {
                    new Models.KsbTaxonomyItem("12", 99),
                    new Models.KsbTaxonomyItem("15", 55),
                }),
                DateOnly.FromDateTime(DateTime.Now)));

            db.Progress.Add(Models.Progress.CreateWithDate(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.MinValue),
                new Models.ApprovalId(1, null),
                new Models.KsbTaxonomy(new[]
                {
                    new Models.KsbTaxonomyItem("12", 88),
                    new Models.KsbTaxonomyItem("14", 44),
                }),
                DateOnly.FromDateTime(DateTime.Now.AddDays(-1))));

            return db.SaveChangesAsync();
        });

        var response = await Client.PostAsync("/apprenticeship/1/snapshot", null);

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