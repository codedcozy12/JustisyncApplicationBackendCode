using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructures.Configuration.EntityTypeConfiguration
{
    public class UserCourtSessionConfiguration : IEntityTypeConfiguration<UserCourtSession>
    {
        public void Configure(EntityTypeBuilder<UserCourtSession> builder)
        {

            builder.ToTable("UserCourtSessions");

            builder.HasKey(ucs => new { ucs.UserId, ucs.CourtSessionId });

            builder.HasOne(ucs => ucs.User)
                   .WithMany(u => u.UserCourtSessions)
                   .HasForeignKey(ucs => ucs.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ucs => ucs.CourtSession)
                   .WithMany(cs => cs.UserCourtSessions)
                   .HasForeignKey(ucs => ucs.CourtSessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}