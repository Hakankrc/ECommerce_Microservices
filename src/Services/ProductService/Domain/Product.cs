namespace ProductService.Domain;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty; // Resim yolu
    public int Stock { get; set; }
    
    // CreatedAt gibi alanlar da eklenebilir ama şimdilik sade tutalım.
}