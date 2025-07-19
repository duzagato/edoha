using Edoha.Domain.Interfaces.Context;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Services;
using Edoha.Infraestructure.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Edoha.Application;

public static class ServiceInjection
{
    public static IServiceCollection Register(IServiceCollection services)
    {
        services.AddScoped<IRequestValidationContext, RequestValidationContext>();
        services.AddScoped<ILotteryService, LotteryService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<ITicketbookService, TicketbookService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserTypeService, UserTypeService>();

        return services;
    }
}
