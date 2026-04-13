using BlindMatch.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Supervisor> Supervisors { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
    public DbSet<ResearchArea> ResearchAreas { get; set; }
    public DbSet<SupervisorInterest> SupervisorInterests { get; set; }
    // Other entities will be added by other members

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply configurations
        // Configurations will be added by other members as they implement entities
    }
}
