using MediatR;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using SFA.DAS.TrackProgress.Api;
using SFA.DAS.TrackProgress.Api.AppStart;
using SFA.DAS.TrackProgress.Api.Configuration;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguration();
IConfiguration configuration = builder.Configuration;

var appConfig = configuration.Get<TrackProgressConfiguration>();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<AzureActiveDirectoryConfiguration>(configuration.GetSection("AzureAd"));
builder.Services.AddSingleton(cfg => cfg.GetRequiredService<IOptions<AzureActiveDirectoryConfiguration>>().Value);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.UseMonthYearTypeConverter());
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddApiAuthentication(appConfig.AzureAd);
builder.Services
    .AddControllers(options =>
    {
        options.UseMonthYearTypeConverter();
        if (!builder.Configuration.IsLocalAcceptanceOrDev())
        {
            options.Filters.Add(new AuthorizeFilter(PolicyNames.Default));
        }
    }).AddJsonOptions(options => options.UseMonthYearTypeConverter());

if(!configuration.IsAcceptanceTest())
    builder.Services.AddEntityFrameworkForTrackProgress(appConfig.ApplicationSettings.DbConnectionString);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddMediatR(typeof(TrackProgressContext));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionPipelineBehavior<,>));

builder.Services.AddTrackProgressHealthChecks();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection()
    .UseAuthentication();

app.MapControllers();
app.UseHealthChecks();
app.Run();

public partial class Program
{ }