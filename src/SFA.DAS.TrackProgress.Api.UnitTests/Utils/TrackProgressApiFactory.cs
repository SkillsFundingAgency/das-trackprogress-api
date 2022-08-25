using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.TrackProgress.Api.Tests.Utils;
using SFA.DAS.TrackProgress.Database;

namespace SFA.DAS.TrackProgress.Api.UnitTests.Utils;

public class TrackProgressApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            UseInMemoryDatabase(services);
        });
        builder.UseEnvironment("LOCAL_ACCEPTANCE_TESTS");
    }

    private static void UseInMemoryDatabase(IServiceCollection services)
    {
        services.Remove<DbContextOptions<TrackProgressContext>>();
        services.AddDbContext<TrackProgressContext>(options => options.UseInMemoryDatabase("db"));
    }
}