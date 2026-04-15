using BlindMatch.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindMatch.Infrastructure.Data.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.StudentId).IsRequired();
        builder.Property(m => m.SupervisorId).IsRequired();
        builder.Property(m => m.Status).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();

        // A proposal can only have one match ever
        builder.HasIndex(m => m.ProposalId).IsUnique();

        builder.HasOne(m => m.Proposal)
               .WithMany()
               .HasForeignKey(m => m.ProposalId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Student)
               .WithMany()
               .HasForeignKey(m => m.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Supervisor)
               .WithMany()
               .HasForeignKey(m => m.SupervisorId)
               .OnDelete(DeleteBehavior.Restrict);

        // One-to-one with IdentityReveal FK lives on IdentityReveal
        builder.HasOne(m => m.IdentityReveal)
               .WithOne(r => r.Match)
               .HasForeignKey<IdentityReveal>(r => r.MatchId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
