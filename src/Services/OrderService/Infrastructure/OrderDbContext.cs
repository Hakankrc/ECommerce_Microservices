using Microsoft.EntityFrameworkCore;
using OrderService.Domain; 

namespace OrderService.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Set decimal precision for TotalPrice to avoid truncation
    modelBuilder.Entity<OrderService.Domain.Order>()
        .Property(o => o.TotalPrice)
        .HasColumnType("decimal(18,2)");
}
}

