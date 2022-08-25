using MediatR;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.TrackProgress.Api;
using SFA.DAS.TrackProgress.Api.AppStart;
using SFA.DAS.TrackProgress.Api.Configuration;
using SFA.DAS.TrackProgress.Database;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguration();
IConfiguration configuration = builder.Configuration;
var config = new ConfigurationBuilder()
    .AddConfiguration(configuration)
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddEnvironmentVariables();

if (!configuration.IsAcceptanceOrDev())
{
    config.AddAzureTableStorage(options =>
    {
        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
        options.EnvironmentName = configuration["EnvironmentName"];
        options.PreFixConfigurationKeys = false;
    });
}

configuration = config.Build();

var appConfig = configuration.Get<TrackProgressConfiguration>();

builder.Services.AddApplicationInsightsTelemetry();
// Add services to the container.
builder.Services.AddControllers(o =>
    {
        if (!builder.Configuration.IsLocalAcceptanceOrDev())
        {
            //o.Filters.Add(new AuthorizeFilter(PolicyNames.Default));
        }
    }
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.UseMonthYearTypeConverter());
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
builder.Services
    .AddControllers(options => options.UseMonthYearTypeConverter())
    .AddJsonOptions(options => options.UseMonthYearTypeConverter())
    ;
builder.Services.AddApiAuthentication(appConfig.AzureAd);
builder.Services.AddDbContext<TrackProgressContext>(options =>
    options.UseSqlServer(appConfig.ApplicationSettings.DbConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddMediatR(typeof(TrackProgressContext));
builder.Services.AddTrackProgressHealthChecks();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks();

app.Run();

public partial class Program
{ }