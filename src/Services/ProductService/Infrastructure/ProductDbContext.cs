using Microsoft.EntityFrameworkCore;
using ProductService.Domain;

namespace ProductService.Infrastructure;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Set decimal precision for Price to avoid truncation
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");
            
        base.OnModelCreating(modelBuilder);
    }
}