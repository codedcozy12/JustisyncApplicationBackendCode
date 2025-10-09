using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class AiChatMessageConfiguration : IEntityTypeConfiguration<AiChatMessage>
    {
        public void Configure(EntityTypeBuilder<AiChatMessage> builder)
        {
            builder.ToTable("AIChatMessages");

            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.AIConversationSession)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.AIConversationSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            builder.Property(m => m.Sender)
                    .HasConversion<string>()
                    .IsRequired();

            builder.Property(m => m.Message)
                   .IsRequired()
                   .HasMaxLength(3000);

            builder.Property(m => m.SentAt)
                   .IsRequired();
        }
    }
}