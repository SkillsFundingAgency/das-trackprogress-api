using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TrackProgress.Api.Tests.Utils;

namespace SFA.DAS.TrackProgress.Api.Tests;

public class ApiFixture
{
    private TrackProgressApiFactory factory = null!;
    private IServiceScopeFactory scopeFactory;
    private protected Fixture fixture = null!;
    private protected HttpClient client = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        factory = new TrackProgressApiFactory();
        scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        client = factory.CreateClient();
    }

    [SetUp]
    public void Setup()
    {
        fixture = new Fixture();
    }

    protected async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = scopeFactory.CreateScope();
        await action(scope.ServiceProvider);
    }

    public Task ExecuteDbContextAsync(Func<TrackProgressContext, Task> action)
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<TrackProgressContext>()));

    protected Task VerifyDatabase(Action<TrackProgressContext> action)
        => ExecuteScopeAsync(sp =>
        {
            action(sp.GetRequiredService<TrackProgressContext>());
            return Task.CompletedTask;
        });

}