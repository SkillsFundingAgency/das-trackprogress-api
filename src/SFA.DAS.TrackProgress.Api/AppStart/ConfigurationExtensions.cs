namespace SFA.DAS.TrackProgress.Api.AppStart;

public static class ConfigurationExtensions
{
    public static bool IsAcceptanceTest(this IConfiguration config)
    {
        return config["EnvironmentName"].Equals("ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsAcceptanceOrDev(this IConfiguration config)
    {
        return config["EnvironmentName"].Equals("ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase) ||
               config["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsLocalAcceptanceOrDev(this IConfiguration config)
    {
        return config["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
               config["EnvironmentName"].Equals("ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase) ||
               config["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }
}
