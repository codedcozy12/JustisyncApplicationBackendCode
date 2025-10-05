using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class CaseProceedingRecordConfiguration : IEntityTypeConfiguration<CaseProceedingRecord>
    {
        public void Configure(EntityTypeBuilder<CaseProceedingRecord> builder)
        {
            builder.ToTable("CaseProceedingRecords");

            builder.HasKey(cpr => new { cpr.CaseId, cpr.CourtSessionId });

            builder.Property(cpr => cpr.Type)
                   .IsRequired();

            builder.Property(cpr => cpr.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(cpr => cpr.Description)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(cpr => cpr.Status)
                   .IsRequired();

            builder.Property(cpr => cpr.ScheduledDate)
                   .IsRequired();

            builder.Property(cpr => cpr.CompletedDate)
                   .IsRequired(false);

            builder.Property(cpr => cpr.Outcome)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(cpr => cpr.RecordingLink)
                   .HasMaxLength(500);

            builder.Property(cpr => cpr.SessionLink)
                   .HasMaxLength(500);

            builder.Property(cpr => cpr.RecordedByUserId)
                   .IsRequired();

            builder.HasOne(cpr => cpr.Case)
                   .WithMany(c => c.Proceedings)
                   .HasForeignKey(cpr => cpr.CaseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cpr => cpr.CourtSession)
                   .WithMany(cs => cs.Proceedings)
                   .HasForeignKey(cpr => cpr.CourtSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}