using System.Data.Common;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.TrackProgress.Database;

namespace SFA.DAS.TrackProgress.Api.AppStart;

public static class EntityFrameworkExtensions
{
    public static IServiceCollection AddEntityFrameworkForTrackProgress(this IServiceCollection services, string sqlConnection)
    {

        services.AddTransient<IConnectionFactory, SqlServerConnectionFactory>();
        services.AddTransient<IManagedIdentityTokenProvider, ManagedIdentityTokenProvider>();

        return services.AddScoped(p =>
        {
            var connectionFactory = p.GetRequiredService<IConnectionFactory>();

            var optionsBuilder = new DbContextOptionsBuilder<TrackProgressContext>()
                .UseDataStorage(connectionFactory, sqlConnection);
            var dbContext = new TrackProgressContext(optionsBuilder.Options);

            return dbContext;
        });
    }
}

public static class DbContextBuilderExtensions
{
    public static DbContextOptionsBuilder<TContext> UseDataStorage<TContext>(
        this DbContextOptionsBuilder<TContext> builder, IConnectionFactory connectionFactory, string connection)
        where TContext : DbContext
    {
        return connectionFactory.AddConnection(builder, connection);
    }

    public static DbContextOptionsBuilder<TContext> UseDataStorage<TContext>(
        this DbContextOptionsBuilder<TContext> builder, IConnectionFactory connectionFactory, DbConnection connection)
        where TContext : DbContext
    {
        return connectionFactory.AddConnection(builder, connection);
    }

    public static DbContextOptionsBuilder<TContext> UseLocalSqlLogger<TContext>(
        this DbContextOptionsBuilder<TContext> builder, ILoggerFactory loggerFactory, IConfiguration config)
        where TContext : DbContext
    {
        if (config.IsLocalAcceptanceOrDev())
        {
            builder.EnableSensitiveDataLogging().UseLoggerFactory(loggerFactory);
        }

        return builder;
    }
}

public class SqlServerConnectionFactory : IConnectionFactory
{
    private readonly IConfiguration _configuration;
    private readonly IManagedIdentityTokenProvider _managedIdentityTokenProvider;

    public SqlServerConnectionFactory(IConfiguration configuration, IManagedIdentityTokenProvider managedIdentityTokenProvider)
    {
        _configuration = configuration;
        _managedIdentityTokenProvider = managedIdentityTokenProvider;
    }

    public DbContextOptionsBuilder<TContext> AddConnection<TContext>(DbContextOptionsBuilder<TContext> builder, string connection) where TContext : DbContext
    {
        return builder.UseSqlServer(CreateConnection(connection));
    }

    public DbContextOptionsBuilder<TContext> AddConnection<TContext>(DbContextOptionsBuilder<TContext> builder, DbConnection connection) where TContext : DbContext
    {
        return builder.UseSqlServer(connection);
    }

    public DbConnection CreateConnection(string connection)
    {
        var sqlConnection = new SqlConnection(connection)
        {
            AccessToken = GetAccessToken(),
        };

        return sqlConnection;
    }

    private string? GetAccessToken()
    {
        if (_configuration.IsLocalAcceptanceOrDev())
        {
            return null;
        }

        return _managedIdentityTokenProvider.GetSqlAccessTokenAsync()
            .GetAwaiter().GetResult();
    }
}

public interface IConnectionFactory
{
    DbContextOptionsBuilder<TContext> AddConnection<TContext>(DbContextOptionsBuilder<TContext> builder, string connection) where TContext : DbContext;
    DbContextOptionsBuilder<TContext> AddConnection<TContext>(DbContextOptionsBuilder<TContext> builder, DbConnection connection) where TContext : DbContext;
    DbConnection CreateConnection(string connection);
}

public interface IManagedIdentityTokenProvider
{
    Task<string> GetSqlAccessTokenAsync();
}

public class ManagedIdentityTokenProvider : IManagedIdentityTokenProvider
{
    public Task<string> GetSqlAccessTokenAsync()
    {
        var provider = new AzureServiceTokenProvider();
        return provider.GetAccessTokenAsync("https://database.windows.net/");
    }
}