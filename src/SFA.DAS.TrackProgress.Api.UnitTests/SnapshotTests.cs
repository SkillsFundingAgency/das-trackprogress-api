using AutoFixture.NUnit3;
using FluentAssertions;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class SnapshotTests : ApiFixture
{
    [Test, AutoData]
    public async Task Save_progress_to_database(long apprenticeshipId)
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
}