using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.TrackProgress.Messages.Events;

namespace SFA.DAS.TrackProgress.Api.AppStart;

public static class NServiceBusExtensions
{
    public static async Task<UpdateableServiceProvider> StartNServiceBus(this UpdateableServiceProvider serviceProvider, IConfiguration configuration)
    {
        var endpointConfiguration = new EndpointConfiguration("SFA.DAS.TrackProgress")
            .UseMessageConventions()
            .UseNewtonsoftJsonSerializer();

        if (configuration.UseLearningTransport())
        {
            endpointConfiguration.UseTransport<LearningTransport>().StorageDirectory(LearningTransportLocal.Folder());
        }
        else
        {
            endpointConfiguration.UseAzureServiceBusTransport(configuration.NServiceBusConnectionString(), r => r.AddRouting());
        }

        if (!string.IsNullOrEmpty(configuration.NServiceBusLicense()))
        {
            endpointConfiguration.License(configuration.NServiceBusLicense());
        }

        var endpoint = await global::NServiceBus.Endpoint.Start(endpointConfiguration);

        serviceProvider.AddSingleton(p => endpoint)
            .AddSingleton<IMessageSession>(p => p.GetRequiredService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();

        return serviceProvider;
    }
}

public static class RoutingSettingsExtensions
{
    public static void AddRouting(this RoutingSettings settings)
    {
        settings.RouteToEndpoint(typeof(NewProgressAddedEvent), QueueNames.NewProgressAdded);
    }
}

public static class QueueNames
{
    public const string NewProgressAdded = "sfa-das-trackprogress-newprogress-added";
}

public static class LearningTransportLocal
{
    private const string LearningTransportStorageDirectory = "LearningTransportStorageDirectory";

    public static string Folder()
    {
        var learningTransportFolder = Environment.GetEnvironmentVariable(LearningTransportStorageDirectory, EnvironmentVariableTarget.Process);
        if (learningTransportFolder == null)
        {
            learningTransportFolder = Path.Combine(
                Directory.GetCurrentDirectory()[
                    ..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)],
                @"src\.learningtransport");
            SetFolder(learningTransportFolder);
        }
        return learningTransportFolder;
    }

    public static void SetFolder(string folder)
    {
        Environment.SetEnvironmentVariable(LearningTransportStorageDirectory, folder, EnvironmentVariableTarget.Process);
    }
}