using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
        public class CourtConfiguration : IEntityTypeConfiguration<Court>
        {
            public void Configure(EntityTypeBuilder<Court> builder)
            {
                // Table
                builder.ToTable("Courts");

                // Primary Key
                builder.HasKey(c => c.Id);

                // Properties
                builder.Property(c => c.Name)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(c => c.Type)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.Property(c => c.Address)
                       .IsRequired()
                       .HasMaxLength(300);

                builder.Property(c => c.City)
                       .IsRequired()
                       .HasMaxLength(150);

                builder.Property(c => c.State)
                       .IsRequired()
                       .HasMaxLength(150);

                builder.Property(c => c.ContactEmail)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(c => c.ContactPhone)
                       .IsRequired()
                       .HasMaxLength(50);

                // Enum mapping (stored as int)
                builder.Property(c => c.Jurisdiction)
                       .IsRequired()
                       .HasConversion<int>();

                // Default value
                builder.Property(c => c.IsActive)
                       .HasDefaultValue(true);

                builder.HasMany(c => c.Registrars)
                       .WithOne(r => r.Court)
                       .HasForeignKey(c => c.CourtId)
                       .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Judges)
                       .WithOne(j => j.Court)
                       .HasForeignKey(j => j.CourtId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(c => c.CourtSessions)
                       .WithOne(cs => cs.Court)
                       .HasForeignKey(cs => cs.CourtId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }