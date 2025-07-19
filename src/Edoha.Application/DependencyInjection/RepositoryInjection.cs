using Edoha.Domain.Interfaces.Repositories;
using Edoha.Infraestructure.Repositories;
using Edoha.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

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
