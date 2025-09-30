using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(n => n.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(n => n.IsRead)
                   .HasDefaultValue(false);

            builder.Property(n => n.ToId)
                   .IsRequired();

            //builder.HasOne(n => n.User)
            //   .WithMany(u => u.Notifications)
            //   .HasForeignKey(n => n.ToId) 
            //   .HasPrincipalKey(u => u.Id) 
            //   .OnDelete(DeleteBehavior.Cascade);


            builder.Property(n => n.DateCreated);

            builder.Property(n => n.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}