using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.ToTable("Courts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ContactEmail)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Location)
            .HasMaxLength(200);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(c => c.Registrars)
            .WithOne(r => r.Court)
            .HasForeignKey(r => r.CourtId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.JudgeCourts)
            .WithOne(jc => jc.Court)
            .HasForeignKey(jc => jc.CourtId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Jurisdictions)
            .WithMany(j => j.Courts)
            .UsingEntity<Dictionary<string, object>>(
                "CourtJurisdictions",
                j => j.HasOne<Jurisdiction>()
                      .WithMany()
                      .HasForeignKey("JurisdictionId")
                      .OnDelete(DeleteBehavior.Cascade),
                c => c.HasOne<Court>()
                      .WithMany()
                      .HasForeignKey("CourtId")
                      .OnDelete(DeleteBehavior.Cascade),
                je =>
                {
                    je.ToTable("CourtJurisdictions");
                    je.HasKey("CourtId", "JurisdictionId");
                });


        builder.HasMany(c => c.CourtSessions)
            .WithOne(cs => cs.Court)
            .HasForeignKey(cs => cs.CourtId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.CourtCases)
            .WithOne(cc => cc.Court)
            .HasForeignKey(cc => cc.CourtId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}