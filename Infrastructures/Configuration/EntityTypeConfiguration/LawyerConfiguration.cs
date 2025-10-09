using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class LawyerConfiguration : IEntityTypeConfiguration<Lawyer>
    {
        public void Configure(EntityTypeBuilder<Lawyer> builder)
        {
            builder.ToTable("Lawyers");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.LawSchoolName)
                .HasMaxLength(100);
            builder.Property(l => l.UniversityName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(l => l.BarMembershipNumber)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(l => l.VerificationStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
            builder.HasMany(l => l.Certificates)
                .WithOne()
                .HasForeignKey(l => l.LawyerId)
                //.HasForeignKey("LawyerId")
                .OnDelete(DeleteBehavior.Cascade);
            //builder.HasMany(l => l.)
            //    .WithOne()
            //    .HasForeignKey("LawyerId")
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}