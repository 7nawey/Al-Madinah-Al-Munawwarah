using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AlMadina.Domain.Entities;


namespace AlMadina.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price)
                    .HasPrecision(18, 2);

                entity.Property(p => p.CostPrice)
                    .HasPrecision(18, 2);

                entity.Property(p => p.DiscountPercentage)
                    .HasPrecision(5, 2);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.TotalPrice)
                    .HasPrecision(18, 2);

                entity.Property(o => o.DeliveryFee)
                    .HasPrecision(18, 2);

                entity.HasMany(o => o.Items)
                    .WithOne(i => i.Order)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(i => i.UnitPrice)
                    .HasPrecision(18, 2);

                entity.Property(i => i.TotalPrice)
                    .HasPrecision(18, 2);
            });

            
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}