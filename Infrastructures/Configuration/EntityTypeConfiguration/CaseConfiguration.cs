using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JustiSync.Infrastructure.Configurations
{
    public class CaseConfiguration : IEntityTypeConfiguration<Case>
    {
        public void Configure(EntityTypeBuilder<Case> builder)
        {
            builder.ToTable("Cases");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CaseNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.OffenseType)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.OffenseLocation)
                .HasMaxLength(255);

            builder.HasOne(c => c.Court)
                .WithMany(court => court.CourtCases)
                .HasForeignKey(c => c.CourtId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Client)
                .WithMany()
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Lawyer)
                .WithMany()
                .HasForeignKey(c => c.LawyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Judge)
                .WithMany(j => j.Cases)
                .HasForeignKey(c => c.JudgeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Registrar)
                .WithMany(a => a.Cases)
                .HasForeignKey(c => c.RegistrarId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.CaseParties)
                .WithOne(cp => cp.Case)
                .HasForeignKey(cp => cp.CaseId);

            builder.HasMany(c => c.Documents)
                .WithOne(d => d.Case)
                .HasForeignKey(d => d.CaseId);

            builder.HasMany(c => c.Proceedings)
                .WithOne(p => p.Case)
                .HasForeignKey(p => p.CaseId);

            builder.HasMany(c => c.CourtSessions)
                .WithOne(cs => cs.Case)
                .HasForeignKey(cs => cs.CaseId);
        }
    }
}