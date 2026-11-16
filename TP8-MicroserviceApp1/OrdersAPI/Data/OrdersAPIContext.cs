using Microsoft.EntityFrameworkCore;

public class OrdersAPIContext(DbContextOptions<OrdersAPIContext> options) : DbContext(options)
{
    public DbSet<OrdersAPI.Orders> Orders { get; set; } = default!;
    public DbSet<OrdersAPI.Product> Products { get; set; }
}
