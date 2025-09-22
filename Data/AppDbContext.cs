using Microsoft.EntityFrameworkCore;
using FintcsApi.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FintcsApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Society> Societies { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<SocietyApproval> SocietyApprovals { get; set; }
        public DbSet<LoanTaken> Loans { get; set; }
        public DbSet<Ledger> Ledgers { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Ensure all DateTimes are stored in UTC
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );
            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                        property.SetValueConverter(dateTimeConverter);

                    if (property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(nullableDateTimeConverter);
                }
            }

            // Configure LoanTaken
            modelBuilder.Entity<LoanTaken>(entity =>
            {
                entity.ToTable("Loans");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.LoanDate).IsRequired();
                entity.Property(l => l.LoanType).IsRequired().HasMaxLength(100);
                entity.Property(l => l.LoanAmount).IsRequired().HasColumnType("numeric(18,2)");
                entity.Property(l => l.PreviousLoan).HasColumnType("numeric(18,2)").HasDefaultValue(0);
                entity.Property(l => l.Installments).IsRequired();
                entity.Property(l => l.Purpose).HasMaxLength(200).HasDefaultValue(string.Empty);
                entity.Property(l => l.AuthorizedBy).HasMaxLength(100).HasDefaultValue(string.Empty);
                entity.Property(l => l.PaymentMode).HasMaxLength(20).HasDefaultValue("Cash");
                entity.Property(l => l.Bank).HasMaxLength(50);
                entity.Property(l => l.ChequeNo).HasMaxLength(50);
                entity.Property(l => l.NetLoan).HasColumnType("numeric(18,2)").HasDefaultValue(0);
                entity.Property(l => l.InstallmentAmount).HasColumnType("numeric(18,2)").HasDefaultValue(0);
                entity.Property(l => l.NewLoanShare).HasColumnType("numeric(18,2)").HasDefaultValue(0);
                entity.Property(l => l.PayAmount).HasColumnType("numeric(18,2)").HasDefaultValue(0);

                entity.Property(l => l.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username).HasMaxLength(50);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(u => u.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Society
            modelBuilder.Entity<Society>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.SocietyName).HasMaxLength(100);
                entity.Property(s => s.Email).HasMaxLength(100);
                entity.Property(s => s.Phone).HasMaxLength(20);
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(s => s.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
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
                .Where(e => e.Entity is User &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.Entity is User user)
                {
                    if (entityEntry.State == EntityState.Added)
                        user.CreatedAt = DateTime.UtcNow;

                    user.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}



















// using Microsoft.EntityFrameworkCore;
// using FintcsApi.Models;
// using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// namespace FintcsApi.Data
// {
//     public class AppDbContext : DbContext
//     {
//         public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//         public DbSet<User> Users { get; set; }
//         public DbSet<Society> Societies { get; set; }
//         public DbSet<LoanType> LoanTypes { get; set; }  
//         public DbSet<Member> Members { get; set; }
//         public DbSet<SocietyApproval> SocietyApprovals { get; set; }
//         public DbSet<LoanTaken> Loans { get; set; }
//         // public DbSet<Demand> Demands { get; set; }


//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);

//             // ✅ Ensure all DateTimes are stored in UTC
//             var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
//                 v => v.ToUniversalTime(),
//                 v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
//             );
//             var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
//                 v => v.HasValue ? v.Value.ToUniversalTime() : v,
//                 v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
//             );

//             foreach (var entityType in modelBuilder.Model.GetEntityTypes())
//             {
//                 foreach (var property in entityType.GetProperties())
//                 {
//                     if (property.ClrType == typeof(DateTime))
//                         property.SetValueConverter(dateTimeConverter);

//                     if (property.ClrType == typeof(DateTime?))
//                         property.SetValueConverter(nullableDateTimeConverter);
//                 }
//             }

//             // Configure LoanTaken
//             modelBuilder.Entity<LoanTaken>(entity =>
//             {
//                 entity.ToTable("Loans");
//                 entity.HasKey(l => l.Id);

//                 entity.Property(l => l.LoanNo).IsRequired().HasMaxLength(50);
//                 entity.Property(l => l.LoanDate).IsRequired();
//                 entity.Property(l => l.LoanType).IsRequired().HasMaxLength(100);
//                 entity.Property(l => l.CustomType).HasMaxLength(100);
//                 entity.Property(l => l.LoanAmount).IsRequired().HasColumnType("numeric(18,2)");
//                 entity.Property(l => l.PreviousLoan).HasColumnType("numeric(18,2)").HasDefaultValue(0);
//                 entity.Property(l => l.Installments).IsRequired();
//                 entity.Property(l => l.Purpose).HasMaxLength(200).HasDefaultValue(string.Empty);
//                 entity.Property(l => l.AuthorizedBy).HasMaxLength(100).HasDefaultValue(string.Empty);
//                 entity.Property(l => l.PaymentMode).HasMaxLength(20).HasDefaultValue("Cash");
//                 entity.Property(l => l.Bank).HasMaxLength(50);
//                 entity.Property(l => l.ChequeNo).HasMaxLength(50);
//                 entity.Property(l => l.NetLoan).HasColumnType("numeric(18,2)").HasDefaultValue(0);
//                 entity.Property(l => l.InstallmentAmount).HasColumnType("numeric(18,2)").HasDefaultValue(0);
//                 entity.Property(l => l.NewLoanShare).HasColumnType("numeric(18,2)").HasDefaultValue(0);
//                 entity.Property(l => l.PayAmount).HasColumnType("numeric(18,2)").HasDefaultValue(0);

//                 // ✅ Use database current timestamp in UTC
//                 entity.Property(l => l.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
//             });

//             // Configure User
//             modelBuilder.Entity<User>(entity =>
//             {
//                 entity.HasKey(u => u.Id);
//                 entity.HasIndex(u => u.Username).IsUnique();
//                 entity.Property(u => u.Username).HasMaxLength(50);
//                 entity.Property(u => u.PasswordHash).IsRequired();
//                 entity.Property(u => u.Details).HasDefaultValue("{}");
//                 entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
//                 entity.Property(u => u.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
//             });

//             // Configure Society
//             modelBuilder.Entity<Society>(entity =>
//             {
//                 entity.HasKey(s => s.Id);
//                 entity.Property(s => s.SocietyName).HasMaxLength(100);
//                 entity.Property(s => s.Email).HasMaxLength(100);
//                 entity.Property(s => s.Phone).HasMaxLength(20);
//                 entity.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
//                 entity.Property(s => s.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
//             });
//         }

//         public override int SaveChanges()
//         {
//             UpdateTimestamps();
//             return base.SaveChanges();
//         }

//         public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//         {
//             UpdateTimestamps();
//             return await base.SaveChangesAsync(cancellationToken);
//         }

//         private void UpdateTimestamps()
//         {
//             var entries = ChangeTracker.Entries()
//                 .Where(e => e.Entity is User &&
//                            (e.State == EntityState.Added || e.State == EntityState.Modified));

//             foreach (var entityEntry in entries)
//             {
//                 if (entityEntry.Entity is User user)
//                 {
//                     if (entityEntry.State == EntityState.Added)
//                         user.CreatedAt = DateTime.UtcNow;

//                     user.UpdatedAt = DateTime.UtcNow;
//                 }
//             }
//         }
//     }
// }
