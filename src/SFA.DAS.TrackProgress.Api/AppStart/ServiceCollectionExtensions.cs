using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Azure.Identity;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Endpoint = Microsoft.AspNetCore.Http.Endpoint;

namespace SFA.DAS.TrackProgress.Api.AppStart;

public static class ServiceCollectionExtensions
{
    public static IHostBuilder UseServiceBus(this IHostBuilder context, IConfiguration configuration)
    {
        context.UseNServiceBus(c =>
        {
            var endpointConfiguration = new EndpointConfiguration(QueueNames.TrackProgress)
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            endpointConfiguration.SendFailedMessagesTo($"{QueueNames.TrackProgress}-error");
            endpointConfiguration.Conventions()
                .DefiningMessagesAs(IsMessage)
                .DefiningEventsAs(IsEvent)
                .DefiningCommandsAs(IsCommand);

            endpointConfiguration.SendOnly();

            //if (UseLearningTransport(configuration))
            //{
            //    var folder = LearningTransportLocal.Folder();
            //    endpointConfiguration.UseTransport<LearningTransport>().StorageDirectory(folder);
            //}
            //else
            {
                endpointConfiguration.UseAzureServiceBusTransport("coprime2.servicebus.windows.net");
            }

            if (!string.IsNullOrEmpty(configuration["ApplicationSettings:NServiceBusLicense"]))
            {
                endpointConfiguration.License(configuration["ApplicationSettings:NServiceBusLicense"]);
            }

            return endpointConfiguration;
        });



        //var endpointConfiguration = new EndpointConfiguration("SFA.DAS.ApprenticeCommitments.Api")
        //    .UseMessageConventions()
        //    .UseNewtonsoftJsonSerializer();

        //if (UseLearningTransport(configuration))
        //{
        //    endpointConfiguration.UseTransport<LearningTransport>();
        //}
        //else
        //{
        //    endpointConfiguration.UseAzureServiceBusTransport(configuration["ApplicationSettings:NServiceBusConnectionString"]);
        //}


        //serviceProvider.AddSingleton(p => endpoint)
        //    .AddSingleton<IMessageSession>(p => p.GetRequiredService<IEndpointInstance>())
        //    .AddHostedService<NServiceBusHostedService>();

        return context;
    }
    public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config,
        string connectionString, Action<RoutingSettings>? routing = null)
    {
        var options = new DefaultAzureCredentialOptions()
        {

            ExcludeAzurePowerShellCredential = true,
            ExcludeEnvironmentCredential = true,
            ExcludeAzureCliCredential = true,
            ExcludeInteractiveBrowserCredential = true,
            ExcludeManagedIdentityCredential = true,
            ExcludeSharedTokenCacheCredential = true,
            ExcludeVisualStudioCodeCredential = true,
            ExcludeVisualStudioCredential = false
        };

        var cred = new DefaultAzureCredential(options);
        
        var transport = config.UseTransport<AzureServiceBusTransport>();
        //transport.CustomTokenCredential(cred);
        transport.CustomTokenCredential(new VisualStudioCredential());
        //transport.CustomTokenCredential(new DefaultAzureCredential());
        transport.ConnectionString(connectionString.FormatConnectionString());
        transport.Transactions(TransportTransactionMode.ReceiveOnly);
        transport.SubscriptionRuleNamingConvention(AzureQueueNameShortener.Shorten);
        routing?.Invoke(transport.Routing());

        return config;
    }

    public static string FormatConnectionString(this string connectionString)
    {
        return connectionString.Replace("Endpoint=sb://", string.Empty).TrimEnd('/');
    }

    private static bool IsMessage(Type t) => t is IMessage || IsSfaMessage(t, "Messages");

    private static bool IsEvent(Type t) => t is IEvent || IsSfaMessage(t, "Messages.Events");

    private static bool IsCommand(Type t) => t is ICommand || IsSfaMessage(t, "Messages.Commands");

    private static bool IsSfaMessage(Type t, string namespaceSuffix)
        => t.Namespace != null &&
           t.Namespace.StartsWith("SFA.DAS") &&
           t.Namespace.EndsWith(namespaceSuffix);


    //public static async Task<UpdateableServiceProvider> StartNServiceBus(this UpdateableServiceProvider serviceProvider, IConfiguration configuration)
    //{
    //    var endpointConfiguration = new EndpointConfiguration("SFA.DAS.ApprenticeCommitments.Api")
    //        .UseMessageConventions()
    //        .UseNewtonsoftJsonSerializer();

    //    if (UseLearningTransport(configuration))
    //    {
    //        endpointConfiguration.UseTransport<LearningTransport>();
    //    }
    //    else
    //    {
    //        endpointConfiguration.UseAzureServiceBusTransport(configuration["ApplicationSettings:NServiceBusConnectionString"]);
    //    }

    //    if (!string.IsNullOrEmpty(configuration["ApplicationSettings:NServiceBusLicense"]))
    //    {
    //        endpointConfiguration.License(configuration["ApplicationSettings:NServiceBusLicense"]);
    //    }
    //    var endpoint = await global::NServiceBus.Endpoint.Start(endpointConfiguration);

    //    serviceProvider.AddSingleton(p => endpoint)
    //        .AddSingleton<IMessageSession>(p => p.GetRequiredService<IEndpointInstance>())
    //        .AddHostedService<NServiceBusHostedService>();

    //    return serviceProvider;
    //}

    private static bool UseLearningTransport(IConfiguration configuration) =>
        string.IsNullOrEmpty(configuration["ApplicationSettings:NServiceBusConnectionString"]) ||
        configuration["ApplicationSettings:NServiceBusConnectionString"].Equals("UseLearningEndpoint=true",
            StringComparison.CurrentCultureIgnoreCase);



}


public static class LearningTransportLocal
{
    private const string LearningTransportStorageDirectory = "LearningTransportStorageDirectory";

    public static string Folder()
    {
        var learningTransportFolder = Environment.GetEnvironmentVariable(LearningTransportStorageDirectory, EnvironmentVariableTarget.Process);
        if (learningTransportFolder == null)
        {
            var current = Directory.GetCurrentDirectory();
            if (current == null)
            {
                learningTransportFolder = ".learningtransport";
            }

            var pos = Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal);
            if (pos > -1)
            {
                learningTransportFolder = Directory.GetCurrentDirectory()[..pos] + @"src\.learningtransport";
            }
            else
            {
                learningTransportFolder = ".learningtransport";
            }

            //learningTransportFolder = Path.Combine(
            //    Directory.GetCurrentDirectory()[
            //        ..Directory.GetCurrentDirectory().IndexOf("src", StringComparison.Ordinal)],
            //    @"src\.learningtransport");
            SetFolder(learningTransportFolder);
        }
        return learningTransportFolder;
    }

    public static void SetFolder(string folder)
    {
        Environment.SetEnvironmentVariable(LearningTransportStorageDirectory, folder, EnvironmentVariableTarget.Process);
    }
}

public static class EndpointConfigurationExtensions
{
    public static EndpointConfiguration UseNewtonsoftJsonSerializer(
        this EndpointConfiguration config)
    {
        config.UseSerialization<NewtonsoftJsonSerializer>();
        return config;
    }

    public static EndpointConfiguration UseMessageConventions(
        this EndpointConfiguration config)
    {
        ConventionsBuilder conventionsBuilder = config.Conventions();
        conventionsBuilder.DefiningCommandsAs((Func<Type, bool>)(t => Regex.IsMatch(t.Name, "Command(V\\d+)?$") || typeof(DbLoggerCategory.Database.Command).IsAssignableFrom(t)));
        //conventionsBuilder.DefiningEventsAs((Func<Type, bool>)(t => Regex.IsMatch(t.Name, "Event(V\\d+)?$") || typeof(object).IsAssignableFrom(t)));
        conventionsBuilder.DefiningEventsAs((Func<Type, bool>)(t => Regex.IsMatch(t.Name, "Event(V\\d+)?$")));
        return config;
    }

}

public static class QueueNames
{
    public const string TrackProgress = "SFA.DAS.TrackProgress";
}

public static class AzureQueueNameShortener
{
    private const int AzureServiceBusRuleNameMaxLength = 50;
    private const int ShortenedNameHashLength = 8;
    private const int ShortenedNameContextLength =
        AzureServiceBusRuleNameMaxLength - ShortenedNameHashLength - 1;

    /// <summary>
    /// <para>
    /// Convert event names that are too long for Azure to a shorter, but legible, representation
    /// by removing the common namespace identifiers and focussing on the interesting specifics
    /// for a particular event.
    /// </para>
    /// <para>
    /// e.g.
    ///     SFA.DAS.CommitmentsV2.Messages.Events.ApprenticeshipCreatedEvent
    ///     becomes
    ///     CommitmentsV2.ApprenticeshipCreatedEvent
    /// </para>
    /// </summary>
    /// <param name="eventType">The event type to shorten.</param>
    /// <returns></returns>
    public static string Shorten(Type eventType)
    {
        var ruleName = eventType.FullName
                       ?? throw new ArgumentException("Could not find name of eventType");
        var importantName = ruleName.Replace("SFA.DAS.", "").Replace(".Messages.Events", "");

        if (importantName.Length <= AzureServiceBusRuleNameMaxLength)
            return importantName;

        var r = new Regex(@"\b(\w+)$");
        var lastWord = r.Match(importantName).Value;
        if (lastWord.Length > ShortenedNameContextLength)
            lastWord = lastWord[..ShortenedNameContextLength];

#pragma warning disable SYSLIB0021 // Type or member is obsolete
        using var md5 = new SHA512Managed();
#pragma warning restore SYSLIB0021 // Type or member is obsolete
        var hash = md5.ComputeHash(Encoding.Default.GetBytes(ruleName));
        var hashstr = BitConverter.ToString(hash).Replace("-", string.Empty);

        return $"{lastWord}.{hashstr[..ShortenedNameHashLength]}";
    }
}



public static class AutoSubscribeToQueues
{
    public static async Task CreateQueuesWithReflection(
        IConfiguration configuration,
        string connectionStringName = "AzureWebJobsServiceBus",
        string? errorQueue = null,
        string topicName = "bundle-1",
        ILogger? logger = null)
    {
        var connectionString = configuration.GetValue<string>(connectionStringName);
        var managementClient = new ManagementClient(connectionString);
        await CreateQueuesWithReflection(managementClient, errorQueue, topicName, logger);
    }

    public static async Task CreateQueuesWithReflection(
        ManagementClient managementClient,
        string? errorQueue = null,
        string topicName = "bundle-1",
        ILogger? logger = null)
    {
        var attribute = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<FunctionNameAttribute>(false) != null)
            .SelectMany(m => m.GetParameters())
            .SelectMany(p => p.GetCustomAttributes<ServiceBusTriggerAttribute>(false))
            .FirstOrDefault()
            ?? throw new Exception("No endpoint was found");

        var endpointQueueName = attribute.QueueName;

        logger?.LogInformation("Queue Name: {queueName}", endpointQueueName);

        errorQueue ??= $"{endpointQueueName}-error";

        await CreateQueue(endpointQueueName, managementClient, logger);
        await CreateQueue(errorQueue, managementClient, logger);

        await CreateSubscription(topicName, managementClient, endpointQueueName, logger);
    }

    private static async Task CreateQueue(string endpointQueueName, ManagementClient managementClient, ILogger? logger)
    {
        if (await managementClient.QueueExistsAsync(endpointQueueName)) return;

        logger?.LogInformation("Creating queue: `{queueName}`", endpointQueueName);
        await managementClient.CreateQueueAsync(endpointQueueName);
    }

    private static async Task CreateSubscription(string topicName, ManagementClient managementClient, string endpointQueueName, ILogger? logger)
    {
        if (await managementClient.SubscriptionExistsAsync(topicName, endpointQueueName)) return;

        logger?.LogInformation($"Creating subscription to: `{endpointQueueName}`", endpointQueueName);

        var description = new SubscriptionDescription(topicName, endpointQueueName)
        {
            ForwardTo = endpointQueueName,
            UserMetadata = $"Subscribed to {endpointQueueName}"
        };

        var ignoreAllEvents = new RuleDescription { Filter = new FalseFilter() };

        await managementClient.CreateSubscriptionAsync(description, ignoreAllEvents);
    }
}