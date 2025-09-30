using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Registrar> Registrars { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<Judge> Judges { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<CourtSession> CourtSessions { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<CaseProceedingRecord> CaseProceedingRecords { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<AIConversationSession> AIConversationSessions { get; set; }
        public DbSet<AIChatMessage> AIChatMessages { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContext).Assembly);
        }
    }
}
