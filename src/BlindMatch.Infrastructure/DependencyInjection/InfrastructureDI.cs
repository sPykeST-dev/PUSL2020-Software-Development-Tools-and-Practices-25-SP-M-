using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.Interfaces.Services;
using BlindMatch.Infrastructure.Data;
using BlindMatch.Infrastructure.Identity;
using BlindMatch.Infrastructure.Repositories;
using BlindMatch.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlindMatch.Infrastructure.DependencyInjection;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ISupervisorRepository, SupervisorRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IInterestRepository, InterestRepository>();
        services.AddScoped<IProposalRepository, ProposalRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        // Other repositories will be added by other members

        // Services
        services.AddScoped<SupervisorService>();
        services.AddScoped<IProposalService, ProposalService>();
        services.AddScoped<IBlindMatchService, BlindMatchService>();
        services.AddScoped<IIdentityRevealService, IdentityRevealService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuditService, AuditService>();
        // Other services will be added by other members

        return services;
    }
}