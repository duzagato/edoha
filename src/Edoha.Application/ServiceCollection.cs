using Edoha.Application.DependencyInjection;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Infraestructure.Factories;
using Npgsql;
using System.Data;
using System.Runtime.CompilerServices;

namespace Edoha.Application;

public static class ServoceCollection
{
    public static IServiceCollection AddFactories(this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        return services;
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

    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        return JwtInjection.AddJwt(services, configuration);
    }
}
