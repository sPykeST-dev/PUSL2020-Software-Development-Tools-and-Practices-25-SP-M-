using BlindMatch.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Supervisor> Supervisors { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<ResearchArea> ResearchAreas { get; set; }
    public DbSet<SupervisorInterest> SupervisorInterests { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<IdentityReveal> IdentityReveals { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<AuditEvent> AuditEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<ResearchArea>().HasData(
            new ResearchArea { Id = 1, Name = "Artificial Intelligence", IsActive = true },
            new ResearchArea { Id = 2, Name = "Web Development",         IsActive = true },
            new ResearchArea { Id = 3, Name = "Cybersecurity",           IsActive = true },
            new ResearchArea { Id = 4, Name = "Cloud Computing",         IsActive = true },
            new ResearchArea { Id = 5, Name = "Machine Learning",        IsActive = true },
            new ResearchArea { Id = 6, Name = "Data Science",            IsActive = true },
            new ResearchArea { Id = 7, Name = "Networking",              IsActive = true }
        );
    }
}
