using BlindMatch.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindMatch.Infrastructure.Data.Configurations;

public class IdentityRevealConfiguration : IEntityTypeConfiguration<IdentityReveal>
{
    public void Configure(EntityTypeBuilder<IdentityReveal> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RevealedAt).IsRequired();
        builder.Property(r => r.TriggeredBySupervisorId).IsRequired();
        // FK to Match is already configured in MatchConfiguration
    }
}