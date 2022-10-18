using System.Collections.Concurrent;
using System.Formats.Asn1;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TrackProgress.Api.UnitTests.Utils;
using SFA.DAS.TrackProgress.Database;

namespace SFA.DAS.TrackProgress.Api.UnitTests;

public class ApiFixture
{
    private TrackProgressApiFactory _factory = null!;
    private IServiceScopeFactory _scopeFactory;
    private protected Fixture Fixture = null!;
    private protected HttpClient Client = null!;
    private protected ConcurrentBag<object> EventsProvider = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new TrackProgressApiFactory(() => EventsProvider);
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        Client = _factory.CreateClient();
    }

    [SetUp]
    public async Task Setup()
    {
        EventsProvider = new ConcurrentBag<object>();
        Fixture = new Fixture();
        await ResetDatabase();
    }

    private async Task ResetDatabase()
    {
        await ExecuteDbContextAsync(db =>
        {
            db.Progress.RemoveRange(db.Progress);
            db.Snapshot.RemoveRange(db.Snapshot);
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