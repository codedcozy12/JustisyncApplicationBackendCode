using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class RegistrarConfiguration : IEntityTypeConfiguration<Registrar>
    {
        public void Configure(EntityTypeBuilder<Registrar> builder)
        {
            builder.ToTable("Registrars");

            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.Court)
                   .WithMany() 
                   .HasForeignKey(r => r.CourtId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.Property(r => r.CourtId)
                   .IsRequired();

            builder.HasOne(r => r.User)
                   .WithMany() 
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(r => r.UserId)
                   .IsRequired();
        }
    }
}