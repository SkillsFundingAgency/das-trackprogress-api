using Microsoft.EntityFrameworkCore;
using SFA.DAS.TrackProgress;
using SFA.DAS.TrackProgress.Api;
using SFA.DAS.TrackProgress.Api.AppStart;
using SFA.DAS.TrackProgress.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguration();
var config = builder.Configuration.Get<TrackProgressConfiguration>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.UseMonthYearTypeConverter());
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
builder.Services
    .AddControllers(options => options.UseMonthYearTypeConverter())
    .AddJsonOptions(options => options.UseMonthYearJsonConverter())
    ;
builder.Services.AddApiAuthentication(config.AzureAd);
builder.Services.AddDbContext<TrackProgressContext>(options =>
    options.UseSqlServer(config.ApplicationSettings.DbConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
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

app.Run();

public partial class Program
{ }