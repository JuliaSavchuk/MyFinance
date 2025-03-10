using Microsoft.EntityFrameworkCore;
using MyFinance.Models;

namespace MyFinance.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public Action<object, EventArgs> DataChanged { get; internal set; }

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Замініть рядок підключення на ваш власний
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyFinanceDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфігурація для User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Currency).IsRequired().HasMaxLength(3);
                entity.HasMany(u => u.Budgets)
                      .WithOne()
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфігурація для Budget
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Limit).HasColumnType("decimal(18,2)");
                entity.Property(b => b.TotalIncome).HasColumnType("decimal(18,2)");
                entity.Property(b => b.TotalExpense).HasColumnType("decimal(18,2)");
                entity.Property(b => b.Balance).HasColumnType("decimal(18,2)");
                entity.HasMany(b => b.Transactions)
                      .WithOne()
                      .HasForeignKey(t => t.BudgetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфігурація для Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Description).HasMaxLength(255);
                entity.Property(t => t.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(t => t.Type).IsRequired();
                entity.Property(t => t.Date).IsRequired();
                entity.HasOne(t => t.Category)
                      .WithMany(c => c.Transactions)
                      .HasForeignKey(t => t.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфігурація для Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Color).HasMaxLength(7); // Наприклад, "#FF5733"
            });

            // Конфігурація для Currency
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Code).IsRequired().HasMaxLength(3);
                entity.Property(c => c.Rate).IsRequired().HasColumnType("decimal(18,4)");
            });
        }
    }
}