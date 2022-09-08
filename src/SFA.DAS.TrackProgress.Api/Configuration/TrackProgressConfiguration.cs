namespace SFA.DAS.TrackProgress.Api.Configuration;

public class TrackProgressConfiguration
{
    public ApplicationSettings ApplicationSettings { get; set; } = new();
    public AzureActiveDirectoryConfiguration AzureAd { get; set; } = new();
}

public class ApplicationSettings
{
    public string DbConnectionString { get; set; } = "";
}