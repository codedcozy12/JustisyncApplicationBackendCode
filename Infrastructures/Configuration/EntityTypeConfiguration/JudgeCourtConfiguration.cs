using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class JudgeCourtConfiguration : IEntityTypeConfiguration<JudgeCourt>
    {
        public void Configure(EntityTypeBuilder<JudgeCourt> builder)
        {
            builder.ToTable("JudgeCourts");

            builder.HasKey(jc => new { jc.JudgeId, jc.CourtId });

            builder.HasOne(jc => jc.Judge)
                   .WithMany(j => j.JudgeCourts)
                   .HasForeignKey(jc => jc.JudgeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(jc => jc.Court)
                   .WithMany(c => c.JudgeCourts)
                   .HasForeignKey(jc => jc.CourtId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}