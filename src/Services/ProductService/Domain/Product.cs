namespace ProductService.Domain;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty; // Resim yolu
    public int Stock { get; set; }
    
    //  We can also add fields like CreatedAt, but let's keep it simple for now.
}