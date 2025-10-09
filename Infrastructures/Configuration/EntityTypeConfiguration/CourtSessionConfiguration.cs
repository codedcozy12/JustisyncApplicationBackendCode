using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class CourtSessionConfiguration : IEntityTypeConfiguration<CourtSession>
    {
        public void Configure(EntityTypeBuilder<CourtSession> builder)
        {
            builder.ToTable("CourtSessions");

            builder.HasKey(cs => cs.Id);

            builder.Property(cs => cs.ScheduledDate)
                   .IsRequired();

            builder.Property(cs => cs.StartTime)
                   .IsRequired(false);

            builder.Property(cs => cs.EndTime)
                   .IsRequired(false);

            builder.Property(cs => cs.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(30);

            builder.Property(cs => cs.Outcome)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(cs => cs.Case)
                   .WithMany(c => c.CourtSessions)
                   .HasForeignKey(cs => cs.CaseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.Court)
                   .WithMany(c => c.CourtSessions)
                   .HasForeignKey(cs => cs.CourtId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cs => cs.Judge)
                   .WithMany()
                   .HasForeignKey(cs => cs.JudgeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(cs => cs.Proceedings)
                   .WithOne(p => p.CourtSession)
                   .HasForeignKey(p => p.CourtSessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(cs => cs.Users)
                   .WithMany()
                   .UsingEntity<Dictionary<string, object>>(
                        "CourtSessionUser",
                        j => j.HasOne<User>()
                              .WithMany()
                              .HasForeignKey("UserId")
                              .OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<CourtSession>()
                              .WithMany()
                              .HasForeignKey("CourtSessionId")
                              .OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.ToTable("CourtSessionUsers");
                            j.HasKey("CourtSessionId", "UserId");
                        }
                   );

            builder.HasIndex(cs => new { cs.JudgeId, cs.ScheduledDate })
                   .IsUnique()
                   .HasDatabaseName("IX_Judge_ScheduledDate_Unique");
        }
    }
}
