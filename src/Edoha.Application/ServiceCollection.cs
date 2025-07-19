using Edoha.Application.DependencyInjection;
using Npgsql;
using System.Data;

namespace Edoha.Application;

public static class ServoceCollection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSingleton<IDbConnection>(sp =>
            new NpgsqlConnection(configuration.GetConnectionString("Default")));
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return RepositoryInjection.Register(services);
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return ServiceInjection.Register(services);
    }

    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        return UtilsInjection.Register(services);
    }
}
