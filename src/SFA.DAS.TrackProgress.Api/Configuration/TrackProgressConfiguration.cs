namespace SFA.DAS.TrackProgress.Api.Configuration;

public class TrackProgressConfiguration
{
    public AzureActiveDirectoryConfiguration AzureAd { get; set; } = new();
}
