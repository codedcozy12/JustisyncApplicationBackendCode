using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.FileName)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(d => d.FileType)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(d => d.FileUrl)
                .IsRequired()
                .HasMaxLength(500);
            builder.HasOne(d => d.Case)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.CaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
