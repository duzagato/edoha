using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infraestructure.Repositories;

namespace Edoha.Application.DependencyInjection;

public static class RepositoryInjection
{
    public static IServiceCollection Register(IServiceCollection services)
    {
        services.AddScoped<IActionRepository, ActionRepository>();
        services.AddScoped<IInstitutionRepository, InstitutionRepository>();
        services.AddScoped<ILoginRepository, LoginRepository>();
        services.AddScoped<ILotteryRepository, LotteryRepository>();
        services.AddScoped<IPageRepository, PageRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITicketbookRepository, TicketbookRepository>();
        services.AddScoped<IStatusTicketbookRepository, StatusTicketbookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
        services.AddScoped<IUserTypeRepository, UserTypeRepository>();

        return services;
    }
}
