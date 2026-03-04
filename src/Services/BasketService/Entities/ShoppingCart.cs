namespace BasketService.Entities;

/// <summary>
/// Represents the user's shopping cart (basket).
/// Acts as the Aggregate Root for the basket context.
/// </summary>
public class ShoppingCart
{
    // The UserName acts as the unique Key for Redis storage.
    public string UserName { get; set; } = string.Empty; 
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();

    public ShoppingCart()
    {
    }

    public ShoppingCart(string userName)
    {
        UserName = userName;
    }

    /// <summary>
    /// Calculates the total price of the cart dynamically.
    /// No need to store this in Redis; it's calculated on the fly.
    /// </summary>
    public decimal TotalPrice
    {
        get
        {
            decimal totalprice = 0;
            foreach (var item in Items)
            {
                totalprice += item.Price * item.Quantity;
            }
            return totalprice;
        }
    }
}