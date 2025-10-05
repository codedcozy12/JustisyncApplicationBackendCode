using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");
      
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Role)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(a => a.Action)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(a => a.EntityName)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(a => a.Details)
                   .HasMaxLength(2000);
            builder.Property(a => a.Timestamp)
                   .IsRequired();
        
            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.EntityId);
        }
    }
}
