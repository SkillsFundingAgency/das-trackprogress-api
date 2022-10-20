using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Testing;
using SFA.DAS.TrackProgress.Api.UnitTests.Utils;
using SFA.DAS.TrackProgress.Database;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class ApiFixture
{
    private TrackProgressApiFactory _factory = null!;
    private IServiceScopeFactory _scopeFactory;
    protected Fixture Fixture = null!;
    protected HttpClient Client = null!;
    protected TestableMessageSession Messages = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new(() => Messages);
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        Client = _factory.CreateClient();
    }

    [SetUp]
    public async Task Setup()
    {
        Fixture = new();
        Messages = new();
        await ResetDatabase();
    }

    private async Task ResetDatabase()
    {
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.RemoveRange(db.Progress);
            db.Snapshot.RemoveRange(db.Snapshot);
            db.KsbCache.RemoveRange(db.KsbCache);
            return db.SaveChangesAsync();
        });
    }

    protected async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        await action(scope.ServiceProvider);
    }

    protected Task ExecuteDbContextAsync(Func<TrackProgressContext, Task> action)
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<TrackProgressContext>()));

    protected Task VerifyDatabase(Action<TrackProgressContext> action)
        => ExecuteScopeAsync(sp =>
        {
            action(sp.GetRequiredService<TrackProgressContext>());
            return Task.CompletedTask;
        });
}