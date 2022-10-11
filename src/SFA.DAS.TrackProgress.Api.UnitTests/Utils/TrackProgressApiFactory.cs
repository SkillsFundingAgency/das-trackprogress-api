using System.Collections.Concurrent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.TrackProgress.Database;

namespace SFA.DAS.TrackProgress.Api.UnitTests.Utils;

public class TrackProgressApiFactory : WebApplicationFactory<Program>
{
    private readonly Func<ConcurrentBag<object>> _events;

    public TrackProgressApiFactory(Func<ConcurrentBag<object>> events)
    {
        _events = events;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("EnvironmentName", "ACCEPTANCE_TESTS");

        builder.ConfigureTestServices(services =>
        {
            UseInMemoryDatabase(services);
            services.AddTransient<IMessageSession>(_ => new FakeMessageSession(_events.Invoke()));
        });
        builder.UseEnvironment("Development");
    }

    private static void UseInMemoryDatabase(IServiceCollection services)
    {
        services.AddDbContext<TrackProgressContext>(options => options.UseInMemoryDatabase("db"));
    }
}

public class FakeMessageSession : IMessageSession
{
    public readonly ConcurrentBag<object> EventsPublished;

    public FakeMessageSession(ConcurrentBag<object> eventsPublished)
    {
        EventsPublished = eventsPublished;
    }

    public Task Send(object message, SendOptions options)
    {
        throw new NotImplementedException();
    }

    public Task Send<T>(Action<T> messageConstructor, SendOptions options)
    {
        throw new NotImplementedException();
    }

    public async Task Publish(object message, PublishOptions options)
    {
        EventsPublished.Add(message);
        await Task.CompletedTask;
    }

    public async Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
    {
        throw new NotImplementedException();
    }

    public Task Subscribe(Type eventType, SubscribeOptions options)
    {
        throw new NotImplementedException();
    }

    public Task Unsubscribe(Type eventType, UnsubscribeOptions options)
    {
        throw new NotImplementedException();
    }
}