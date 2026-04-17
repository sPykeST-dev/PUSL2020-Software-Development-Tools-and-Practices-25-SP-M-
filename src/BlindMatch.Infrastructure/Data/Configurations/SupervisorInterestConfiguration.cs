using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindMatch.Infrastructure.Data.Configurations;

public class SupervisorInterestConfiguration : IEntityTypeConfiguration<SupervisorInterest>
{
    public void Configure(EntityTypeBuilder<SupervisorInterest> builder)
    {
        builder.HasKey(si => si.Id);

        builder.Property(si => si.SupervisorId)
            .IsRequired();

        builder.Property(si => si.ProposalId)
            .IsRequired();

        builder.Property(si => si.Status)
            .IsRequired()
            .HasDefaultValue(InterestStatus.Pending);

        builder.Property(si => si.CreatedAt)
            .IsRequired();

        // Unique constraint to prevent duplicate interests
        builder.HasIndex(si => new { si.SupervisorId, si.ProposalId })
            .IsUnique();

        // Relationships
        builder.HasOne(si => si.Supervisor)
            .WithMany(s => s.Interests)
            .HasForeignKey(si => si.SupervisorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(si => si.Proposal)
            .WithMany(p => p.Interests)
            .HasForeignKey(si => si.ProposalId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
