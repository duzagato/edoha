using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infraestructure.Repositories;

namespace Edoha.Application.DependencyInjection;

public static class RepositoryInjection
{
    public static IServiceCollection Register(IServiceCollection services)
    {
        services.AddScoped<ILotteryRepository, LotteryRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITicketbookRepository, TicketbookRepository>();
        services.AddScoped<IStatusTicketbookRepository, StatusTicketbookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserTypeRepository, UserTypeRepository>();

        return services;
    }
}
