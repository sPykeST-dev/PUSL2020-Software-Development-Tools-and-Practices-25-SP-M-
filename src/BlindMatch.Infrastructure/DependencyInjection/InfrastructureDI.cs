using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Identity;
using BlindMatch.Infrastructure.Repositories;
using BlindMatch.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BlindMatch.Infrastructure.DependencyInjection;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ISupervisorRepository, SupervisorRepository>();
        services.AddScoped<IInterestRepository, InterestRepository>();
        services.AddScoped<IProposalRepository, ProposalRepository>();
        // Other repositories will be added by other members

        // Services
        services.AddScoped<SupervisorService>();
        // Other services will be added by other members

        return services;
    }
}