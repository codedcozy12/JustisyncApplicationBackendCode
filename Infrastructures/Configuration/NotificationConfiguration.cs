using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration
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
                .IsRequired()
                .HasDefaultValue(false);
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.ToId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}