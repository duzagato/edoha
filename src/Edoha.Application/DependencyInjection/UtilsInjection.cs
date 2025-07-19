using Edoha.Domain.Interfaces.Util;
using Edoha.Infraestructure.Util;
using Microsoft.Extensions.DependencyInjection;

namespace Edoha.Application.DependencyInjection;

public static class UtilsInjection
{
    public static IServiceCollection Register(IServiceCollection services)
    {
        services.AddScoped<ICrypto, Crypto>();
        return services;
    }
}
