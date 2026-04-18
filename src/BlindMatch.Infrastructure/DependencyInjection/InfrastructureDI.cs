using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces;
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

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireUppercase       = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireDigit           = true;
            options.Password.RequiredLength         = 8;
            options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddClaimsPrincipalFactory<AppClaimsFactory>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ISupervisorRepository, SupervisorRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IInterestRepository, InterestRepository>();
        services.AddScoped<IProposalRepository, ProposalRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<SupervisorService>();
        services.AddScoped<IProposalService, ProposalService>();
        services.AddScoped<IBlindMatchService, BlindMatchService>();
        services.AddScoped<IIdentityRevealService, IdentityRevealService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IMatchApprovalService, ModuleLeaderService>();
        services.AddScoped<ModuleLeaderService>();
        services.AddScoped<ActiveUserFilter>();

        return services;
    }
}
