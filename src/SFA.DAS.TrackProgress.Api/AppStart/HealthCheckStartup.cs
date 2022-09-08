using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.TrackProgress.Database;

namespace SFA.DAS.TrackProgress.Api.AppStart;

public static class HealthCheckStartup
{
    public static IServiceCollection AddTrackProgressHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
               .AddCheck<TrackProgressHealthCheck>(nameof(TrackProgressHealthCheck));

        return services;
    }

    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health");
        app.UseHealthChecks("/ping", new HealthCheckOptions
        {
            Predicate = (_) => false,
            ResponseWriter = (context, report) =>
            {
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("");
            }
        });
        return app;
    }

    public class TrackProgressHealthCheck : IHealthCheck
    {
        private const string Description = "Track Progress API Health Check";
        private readonly TrackProgressContext _database;

        public TrackProgressHealthCheck(TrackProgressContext database) => _database = database;

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await _database.Progress.FirstOrDefaultAsync(cancellationToken);
                return HealthCheckResult.Healthy(Description);
            }
            catch
            {
                return HealthCheckResult.Unhealthy(Description);
            }
        }
    }
}