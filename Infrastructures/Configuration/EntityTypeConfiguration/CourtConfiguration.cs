using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace JustiSync.Infrastructure.Configurations;
public class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.ToTable("Courts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Type).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Address).IsRequired().HasMaxLength(300);
        builder.Property(c => c.City).IsRequired().HasMaxLength(150);
        builder.Property(c => c.State).IsRequired().HasMaxLength(150);
        builder.Property(c => c.ContactEmail).IsRequired().HasMaxLength(200);
        builder.Property(c => c.ContactPhone).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Location).IsRequired().HasMaxLength(250);
        builder.Property(c => c.IsActive).HasDefaultValue(true);

        builder.HasMany(c => c.Registrars)
            .WithOne(r => r.Court)
            .HasForeignKey(r => r.CourtId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.JudgeCourts)
            .WithOne(jc => jc.Court)
            .HasForeignKey(jc => jc.CourtId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Judges)
            .WithOne(j => j.Court)
            .HasForeignKey(j => j.CourtId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.CourtSessions)
            .WithOne(cs => cs.Court)
            .HasForeignKey(cs => cs.CourtId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.CourtCases)
            .WithOne(ca => ca.Court)
            .HasForeignKey(ca => ca.CourtId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}