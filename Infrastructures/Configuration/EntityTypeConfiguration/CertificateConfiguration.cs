using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.ToTable("Certificates");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CertificateType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.FilePath)
                   .IsRequired();

            builder.Property(c => c.UploadedAt)
                   .IsRequired();

            builder.HasOne(c => c.Lawyer)
                   .WithMany(l => l.Certificates)
                   .HasForeignKey(c => c.LawyerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}