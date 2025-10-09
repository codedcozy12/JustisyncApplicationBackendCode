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
         

            builder.Property(r => r.AppointmentDate)
                   .IsRequired(false);
            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId);

            builder.HasMany(x => x.JudgeCourts)
                .WithOne(x => x.Judge)
                .HasForeignKey(x => x.JudgeId)
                       .OnDelete(DeleteBehavior.Restrict);
                builder.Property(r => r.UserId)
                       .IsRequired();
            }
        }
    }