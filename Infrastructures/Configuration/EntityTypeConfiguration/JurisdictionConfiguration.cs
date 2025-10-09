using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class JurisdictionConfiguration : IEntityTypeConfiguration<Jurisdiction>
    {
        public void Configure(EntityTypeBuilder<Jurisdiction> builder)
        {
            builder.ToTable("Jurisdictions");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.SubjectMatter)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.MonetaryLimit)
                .HasColumnType("decimal(18,2)");
 
        }
    }
}