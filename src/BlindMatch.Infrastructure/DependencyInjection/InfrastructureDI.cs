using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces;
using BlindMatch.Infrastructure.Identity;
using BlindMatch.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BlindMatch.Infrastructure.DependencyInjection;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ActiveUserFilter>();
        
        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsFactory>();

        return services;
    }
}