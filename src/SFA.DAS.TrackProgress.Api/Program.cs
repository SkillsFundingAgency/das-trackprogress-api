using SFA.DAS.TrackProgress.Api.AppStart;
using SFA.DAS.TrackProgress.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguration();
var config = builder.Configuration.Get<TrackProgressConfiguration>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
builder.Services.AddApiAuthentication(config.AzureAd);
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