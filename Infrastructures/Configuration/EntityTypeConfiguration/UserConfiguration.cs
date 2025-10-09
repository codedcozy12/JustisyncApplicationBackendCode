using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username).IsRequired();
            builder.Property(u => u.Email).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.FirstName).IsRequired();
            builder.Property(u => u.LastName).IsRequired();
            builder.Property(u => u.MiddleName).IsRequired();
            builder.Property(u => u.PhoneNumber).IsRequired();

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(u => u.Accountstatus)
                .HasConversion<string>()
                .IsRequired();

            builder.HasMany(u => u.UserChats)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.RecipientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.SentMessages)
                   .WithOne(m => m.Sender)
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.ReceivedMessages)
                   .WithOne(m => m.Receiver)
                   .HasForeignKey(m => m.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.UserCourtSessions)
                   .WithOne(ucs => ucs.User)
                   .HasForeignKey(ucs => ucs.UserId);

            builder.HasMany(u => u.Notifications)
                   .WithOne(n => n.User)
                   .HasForeignKey(n => n.UserId);

            builder.HasMany(u => u.AIConversationSessions)
                   .WithOne(a => a.User)
                   .HasForeignKey(a => a.UserId);
        }
    }
}