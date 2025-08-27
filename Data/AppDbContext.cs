using Microsoft.EntityFrameworkCore;
using FintcsApi.Models;

namespace FintcsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Society> Societies { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username).HasMaxLength(50);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Details).HasDefaultValue("{}");
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(u => u.UpdatedAt).HasDefaultValueSql("datetime('now')");
            });

            // Configure Society entity
            modelBuilder.Entity<Society>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.SocietyName).HasMaxLength(100);
                entity.Property(s => s.Email).HasMaxLength(100);
                entity.Property(s => s.Phone).HasMaxLength(20);
                entity.Property(s => s.Tabs).HasDefaultValue("{}");
                entity.Property(s => s.PendingChanges).HasDefaultValue("{}");
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(s => s.UpdatedAt).HasDefaultValueSql("datetime('now')");
            });

            // Configure Member entity
            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasIndex(m => m.MemNo).IsUnique();
                entity.Property(m => m.MemNo).HasMaxLength(20);
                entity.Property(m => m.Name).HasMaxLength(100);
                entity.Property(m => m.Email).HasMaxLength(100);
                entity.Property(m => m.Mobile).HasMaxLength(20);
                entity.Property(m => m.BankingDetails).HasDefaultValue("{}");
                entity.Property(m => m.PendingChanges).HasDefaultValue("{}");
                entity.Property(m => m.CreatedAt).HasDefaultValueSql("datetime('now')");
                entity.Property(m => m.UpdatedAt).HasDefaultValueSql("datetime('now')");
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => (e.Entity is User || e.Entity is Society || e.Entity is Member) && 
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                switch (entityEntry.Entity)
                {
                    case User user:
                        if (entityEntry.State == EntityState.Added)
                            user.CreatedAt = DateTime.UtcNow;
                        user.UpdatedAt = DateTime.UtcNow;
                        break;
                    
                    case Society society:
                        if (entityEntry.State == EntityState.Added)
                            society.CreatedAt = DateTime.UtcNow;
                        society.UpdatedAt = DateTime.UtcNow;
                        break;
                    
                    case Member member:
                        if (entityEntry.State == EntityState.Added)
                            member.CreatedAt = DateTime.UtcNow;
                        member.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}
