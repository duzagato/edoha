using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Domain.Services;
using Edoha.Infraestructure.Context;
using Edoha.Infraestructure.Services;

namespace Edoha.Application;

public static class ServiceInjection
{
    public static IServiceCollection Register(IServiceCollection services)
    {
        services.AddScoped<ITokenGenerationService, TokenGenerationService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IActionService, ActionService>();
        services.AddScoped<IRequestValidationContext, RequestValidationContext>();
        services.AddScoped<ILotteryService, LotteryService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<ITicketbookService, TicketbookService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        services.AddScoped<IUserTypeService, UserTypeService>();

        return services;
    }
}
