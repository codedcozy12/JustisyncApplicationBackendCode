using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class CasePartyConfiguration : IEntityTypeConfiguration<CaseParty>
    {
        public void Configure(EntityTypeBuilder<CaseParty> builder)
        {

            builder.ToTable("CaseParties");

            builder.HasKey(cp => cp.Id);

            builder.Property(cp => cp.Role)
                   .IsRequired();

            builder.Property(cp => cp.FullName)
                   .HasMaxLength(200);

            builder.Property(cp => cp.ContactEmail)
                   .HasMaxLength(200);

            builder.Property(cp => cp.ContactPhone)
                   .HasMaxLength(50);

            builder.HasOne(cp => cp.Case)
                   .WithMany(c => c.CaseParties)
                   .HasForeignKey(cp => cp.CaseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.User)
                   .WithMany()
                   .HasForeignKey(cp => cp.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
