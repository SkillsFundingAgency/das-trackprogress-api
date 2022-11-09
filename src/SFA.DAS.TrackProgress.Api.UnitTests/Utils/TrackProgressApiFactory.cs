using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.Testing;
using SFA.DAS.TrackProgress.Database;

namespace SFA.DAS.TrackProgress.Api.UnitTests.Utils;

public class TrackProgressApiFactory : WebApplicationFactory<Program>
{
    private readonly Func<TestableMessageSession> _messages;

    public TrackProgressApiFactory(Func<TestableMessageSession> messages)
    {
        _messages = messages;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("EnvironmentName", "ACCEPTANCE_TESTS");

        builder.ConfigureTestServices(services =>
        {
            UseInMemoryDatabase(services);
            services.AddTransient<IMessageSession>(_ => _messages.Invoke());
        });
        builder.UseEnvironment("Development");
    }

    private static void UseInMemoryDatabase(IServiceCollection services)
    {
        services.AddDbContext<TrackProgressContext>(options => options.UseInMemoryDatabase("db"));
    }
}