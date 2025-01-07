using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.TrackProgress.Api.AppStart;

public static class AddConfigurationExtensions
{
    public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder services)
    {
        AddAzureConfiguration(services.Configuration);
        services.Configuration.AddJsonFile($"appsettings.Development.json", optional: true);
        return services;
    }

    public static void AddAzureConfiguration(ConfigurationManager configuration)
    {
        if (configuration.IsLocalAcceptanceOrDev())
            TryAddAzureConfiguration(configuration);
        else
            RequireAddAzureConfiguration(configuration);
    }

    public static void TryAddAzureConfiguration(ConfigurationManager configuration)
    {
        try
        {
            var names = configuration["ConfigNames"]?.Split(",") ?? Array.Empty<string>();
            var connectionString = configuration["ConfigurationStorageConnectionString"];
            var environment = configuration["EnvironmentName"];

            if (names.Length == 0) return;
            if (string.IsNullOrEmpty(connectionString)) return;

            configuration.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = names;
                options.StorageConnectionString = connectionString;
                options.EnvironmentName = environment;
                options.PreFixConfigurationKeys = false;
            });
        }
        catch
        {
            // when running locally we may not be using the storage configuration
        }
    }

    public static void RequireAddAzureConfiguration(ConfigurationManager configuration)
    {
        var names = configuration["ConfigNames"]?.Split(",") ?? Array.Empty<string>();
        var connectionString = configuration["ConfigurationStorageConnectionString"];
        var environment = configuration["EnvironmentName"];

        if (names.Length == 0) return;
        if (string.IsNullOrEmpty(connectionString)) return;

        configuration.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = names;
            options.StorageConnectionString = connectionString;
            options.EnvironmentName = environment;
            options.PreFixConfigurationKeys = false;
        });
    }
}