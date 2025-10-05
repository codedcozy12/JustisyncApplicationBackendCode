using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class JudgeConfiguration : IEntityTypeConfiguration<Judge>
    {
            public void Configure(EntityTypeBuilder<Judge> builder)
            {      
                builder.ToTable("Judges");
                builder.HasKey(r => r.Id);
            builder.Property(r => r.CourtId)
                   .IsRequired();

            builder.Property(r => r.AppointmentDate)
                   .IsRequired(false);
            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId);
            builder.HasOne(x => x.Court)
                .WithMany(x => x.Judges)
                .HasForeignKey(x => x.CourtId)
                       .OnDelete(DeleteBehavior.Restrict);
                builder.Property(r => r.UserId)
                       .IsRequired();
            }
        }
    }