using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class AiConversationSessionConfiguration : IEntityTypeConfiguration<AiConversationSession>
    {
        public void Configure(EntityTypeBuilder<AiConversationSession> builder)
        {
            builder.ToTable("AIConversationSessions");

            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.User)
                   .WithMany(u => u.AIConversationSessions) 
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.StartedAt)
                   .IsRequired();

            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.AIConversationSession)
                   .HasForeignKey(m => m.AIConversationSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}