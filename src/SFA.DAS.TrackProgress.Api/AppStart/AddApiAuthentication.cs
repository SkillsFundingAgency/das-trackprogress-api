using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SFA.DAS.TrackProgress.Api.Configuration;

namespace SFA.DAS.TrackProgress.Api.AppStart;

public static class AddApiAuthenticationExtension
{
    public static void AddApiAuthentication(this IServiceCollection services, AzureActiveDirectoryConfiguration config)
    {
        services.AddAuthorization(o =>
        {
            o.AddPolicy(PolicyNames.Default, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(RoleNames.Default);
            });
        });

        services.AddAuthentication(auth => { auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(auth =>
            {
                auth.Authority =
                    $"https://login.microsoftonline.com/{config.Tenant}";
                auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidAudiences = new List<string>
                    {
                            config.Identifier
                    }
                };
            });
        services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
    }
}

public static class PolicyNames
{
    public static string Default => nameof(Default);
}

public static class RoleNames
{
    public static string Default => nameof(Default);
}
