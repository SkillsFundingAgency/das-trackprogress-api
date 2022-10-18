using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class SnapshotTests : ApiFixture
{
    [Test, AutoData]
    public async Task Save_progress_to_database_1(long apprenticeshipId)
    {
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(new Models.Progress(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.Now.AddDays(-1)),
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
    
    [Test, AutoData]
    public async Task Save_progres_from_single_event(long apprenticeshipId)
    {
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.Add(new Models.Progress(
                new Models.ProviderApprenticeshipIdentifier(1, 1, DateTime.Now.AddDays(-1)),
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
}