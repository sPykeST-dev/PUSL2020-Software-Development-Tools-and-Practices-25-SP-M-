using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlindMatch.Infrastructure.Data.Configurations;

public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
{
	public void Configure(EntityTypeBuilder<Proposal> builder)
	{
		builder.HasKey(p => p.Id);

		builder.Property(p => p.Title)
			.IsRequired()
			.HasMaxLength(150);

		builder.Property(p => p.Abstract)
			.IsRequired()
			.HasMaxLength(2000);

		builder.Property(p => p.TechnicalStack)
			.IsRequired()
			.HasMaxLength(400);

		builder.Property(p => p.Keywords)
			.IsRequired()
			.HasMaxLength(250);

		builder.Property(p => p.StudentId)
			.IsRequired();

		builder.Property(p => p.Status)
			.IsRequired()
			.HasDefaultValue(ProposalStatus.Submitted);

		builder.Property(p => p.UpdatedAt)
			.IsRequired();

		builder.HasIndex(p => p.StudentId)
			.IsUnique();

		builder.HasOne(p => p.ResearchArea)
			.WithMany(ra => ra.Proposals)
			.HasForeignKey(p => p.ResearchAreaId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(p => p.Student)
			.WithOne(s => s.Proposal)
			.HasForeignKey<Proposal>(p => p.StudentId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
