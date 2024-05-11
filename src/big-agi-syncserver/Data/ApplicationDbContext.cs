using Microsoft.EntityFrameworkCore;

namespace big_agi_syncserver.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ConversationFolder> ConversationFolders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Conversation entity
            // Conversation is unique by syncKey and Id
            modelBuilder.Entity<Conversation>()
                .HasAlternateKey(c => new { c.SyncKey, c.Id });
                //.HasKey(c => c.Id);
            
            modelBuilder.Entity<Conversation>()
                .Property(c => c.Id)
                .HasMaxLength(50);
            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId);
            modelBuilder.Entity<Conversation>()
                .Property(c => c.SyncKey)
                .IsRequired()
                .HasMaxLength(36); // assuming guid is stored as string

            // Configure Message entity
            // Message is unique by ConversationId and Id
            modelBuilder.Entity<Message>()
                .HasAlternateKey(c => new { c.ConversationId, c.Id });
                //.HasKey(m => m.Id);
                
            modelBuilder.Entity<Message>()
                .Property(m => m.Id)
                .HasMaxLength(50);
            
            modelBuilder.Entity<Message>().Property(m => m.Avatar).HasMaxLength(100);
            modelBuilder.Entity<Message>().Property(m => m.PurposeId).HasMaxLength(40);
            modelBuilder.Entity<Message>().Property(m => m.Role).HasMaxLength(20);
            modelBuilder.Entity<Message>().Property(m => m.Sender).HasMaxLength(20);
            modelBuilder.Entity<Message>().Property(m => m.Role).HasMaxLength(20);
            
            
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId);
            
            // Configure ConversationFolder entity
            modelBuilder.Entity<ConversationFolder>().HasKey(m => m.Id);
            modelBuilder.Entity<ConversationFolder>().Property(m => m.Id).HasMaxLength(50);
            modelBuilder.Entity<ConversationFolder>().Property(m => m.Title).HasMaxLength(100);
            modelBuilder.Entity<ConversationFolder>().Property(m => m.Color).HasMaxLength(10);
            //modelBuilder.Entity<ConversationFolder>().Property(m => m.Conversations).HasMaxLength(10); // varchar(max)
            
        }
    }
}
