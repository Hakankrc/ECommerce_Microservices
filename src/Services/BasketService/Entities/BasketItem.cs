namespace BasketService.Entities;

/// <summary>
/// Represents a single product line item within the shopping cart.
/// </summary>
public class BasketItem
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    // Initialized to string.Empty to prevent Null Reference warnings.
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty;
}