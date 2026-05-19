using HarcaBak.Entities;
using Microsoft.EntityFrameworkCore;

namespace HarcaBak.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }

        public override int SaveChanges()
        {
            foreach (var item in ChangeTracker.Entries())
            {
                if (item.Entity is BaseEntity entity && item.State == EntityState.Added)
                {
                    entity.CreatedDate = DateTime.Now;
                    entity.CreatedBy = 1;
                }
                if (item.Entity is BaseEntity entity2 && item.State == EntityState.Modified)
                {
                    entity2.UpdatedDate = DateTime.Now;
                    entity2.UpdatedBy = 1;
                }
            }
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.Category)
                .WithMany(category => category.Transactions)
                .HasForeignKey(transaction => transaction.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.User)
                .WithMany(user => user.Transactions)
                .HasForeignKey(transaction => transaction.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
