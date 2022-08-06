using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.TrackProgress.Api.Tests.Utils;

public class TrackProgressApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Remove<DbContextOptions<TrackProgressContext>>();

            var db = Guid.NewGuid().ToString();
            services.AddDbContext<TrackProgressContext>(options => options.UseInMemoryDatabase(db));
        });
    }
}