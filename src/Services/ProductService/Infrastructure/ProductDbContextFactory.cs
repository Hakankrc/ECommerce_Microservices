using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductService.Infrastructure;

/// <summary>
/// This factory is used ONLY by EF Core CLI tools (e.g., 'dotnet ef migrations add').
/// It allows creating migrations without running the full application.
/// </summary>
public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
{
    public ProductDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
        
        // Hardcoded connection string for design-time operations (Migration creation)
        // Ensure this matches your Docker/Local SQL Server credentials
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ProductDb;User Id=sa;Password=Sifre123_Guclu!;TrustServerCertificate=True;");

        return new ProductDbContext(optionsBuilder.Options);
    }
}