using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Produits initiaux
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "High-end gaming laptop",
                    Price = 1500.00m,
                    Stock = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "Smartphone",
                    Description = "Latest model smartphone",
                    Price = 800.00m,
                    Stock = 25
                },
                new Product
                {
                    Id = 3,
                    Name = "Wireless Headphones",
                    Description = "Noise cancelling headphones",
                    Price = 200.00m,
                    Stock = 40
                }
            );
        }
    }
}