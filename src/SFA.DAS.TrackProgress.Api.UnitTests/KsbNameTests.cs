using System.Net.Http.Json;
using FluentAssertions;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class KsbNameTests : ApiFixture
{
    [Test]
    public async Task Save_missing_ksb_names()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.KsbCache.Add(new("6567f034-e675-45a8-aa27-f7b6bcb3a5a1", "original description"));
            return db.SaveChangesAsync();
        });

        // When
        await Client.PostAsJsonAsync("/courses/ksbs", new
        {
            Ksbs = new[]
            {
                new { Id = "3fa85f64-5717-4562-b3fc-2c963f66afa6", Description = "description" },
                new { Id = "94b5e500-2b33-4209-86e6-8de07e9b615f", Description = "more descriptive" }
            }
        });

        // Then
        await VerifyDatabase(db =>
        {
            db.KsbCache.Should().BeEquivalentTo(new[]
            {
                new { Id = "6567f034-e675-45a8-aa27-f7b6bcb3a5a1", Name = "original description" },
                new { Id = "3fa85f64-5717-4562-b3fc-2c963f66afa6", Name = "description" },
                new { Id = "94b5e500-2b33-4209-86e6-8de07e9b615f", Name = "more descriptive" },
            });
        });
    }
    
    [Test]
    public async Task Update_ksb_name()
    {
        // Given
        await ExecuteDbContextAsync(db =>
        {
            db.KsbCache.Add(new("6567f034-e675-45a8-aa27-f7b6bcb3a5a1", "original"));
            return db.SaveChangesAsync();
        });

        // When
        await Client.PostAsJsonAsync("/courses/ksbs", new
        {
            Ksbs = new[]
            {
                new { Id = "6567f034-e675-45a8-aa27-f7b6bcb3a5a1", Description = "replacement" },
            }
        });

        // Then
        await VerifyDatabase(db =>
        {
            db.KsbCache.Should().BeEquivalentTo(new[]
            {
                new { Id = "6567f034-e675-45a8-aa27-f7b6bcb3a5a1", Name = "replacement" },
            });
        });
    }
}