using Edoha.Domain.Interfaces.Infraestructure.Util;
using Edoha.Infraestructure.Util;

namespace Edoha.Application.DependencyInjection;

public static class UtilsInjection
{
    public static IServiceCollection Register(IServiceCollection services)
    {
        services.AddScoped<ICrypto, Crypto>();
        services.AddScoped<IJson, Json>();
        services.AddScoped<ISystemUtils, SystemUtils>();

        return services;
    }
}
