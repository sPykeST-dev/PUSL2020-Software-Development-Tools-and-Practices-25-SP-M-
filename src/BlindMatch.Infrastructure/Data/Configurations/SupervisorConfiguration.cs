using BlindMatch.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindMatch.Infrastructure.Data.Configurations;

public class SupervisorConfiguration : IEntityTypeConfiguration<Supervisor>
{
    public void Configure(EntityTypeBuilder<Supervisor> builder)
    {
        builder.Property(s => s.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.MaxProjects)
            .IsRequired()
            .HasDefaultValue(3);

        builder.Property(s => s.CurrentProjects)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasMany(s => s.PreferredResearchAreas)
            .WithMany(ra => ra.Supervisors)
            .UsingEntity<Dictionary<string, object>>(
                "SupervisorResearchArea",
                j => j.HasOne<ResearchArea>().WithMany().HasForeignKey("ResearchAreaId"),
                j => j.HasOne<Supervisor>().WithMany().HasForeignKey("SupervisorId"));
    }
}
