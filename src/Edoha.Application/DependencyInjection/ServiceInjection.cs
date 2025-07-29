using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Services;
using Edoha.Infraestructure.Context;

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
