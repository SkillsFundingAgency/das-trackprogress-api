using MediatR;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.TrackProgress.Api;
using SFA.DAS.TrackProgress.Api.AppStart;
using SFA.DAS.TrackProgress.Api.Configuration;
using SFA.DAS.TrackProgress.Database;
using SFA.DAS.TrackProgress.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseNServiceBusContainer();

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

if(appConfig != null)
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

if(!configuration.IsAcceptanceTest() && appConfig != null)
    builder.Services.AddEntityFrameworkForTrackProgress(appConfig.ApplicationSettings.DbConnectionString);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddMediatR(typeof(TrackProgressContext));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
if (!configuration.IsAcceptanceTest())
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionPipelineBehavior<,>));

builder.Services.AddTrackProgressHealthChecks();

builder.Host.ConfigureContainer<UpdateableServiceProvider>(sp =>
{
    if (!configuration.IsAcceptanceTest())
        sp.StartNServiceBus(configuration).GetAwaiter().GetResult();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection()
    .UseAuthentication();

app.MapControllers();
app.UseHealthChecks();
await app.RunAsync();

public partial class Program { }