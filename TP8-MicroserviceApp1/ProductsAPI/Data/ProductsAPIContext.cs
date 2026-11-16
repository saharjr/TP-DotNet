using Microsoft.EntityFrameworkCore;

public class ProductsAPIContext(DbContextOptions<ProductsAPIContext> options) : DbContext(options)
{
    public DbSet<ProductsAPI.Product> Product { get; set; } = default!;
}
