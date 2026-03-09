namespace Shared.Events;

public class BasketCheckoutEvent
{
    // Order metadata
    public string UserName { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }

    // Customer and address information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    // Items in the basket (used for stock and order processing)
    public List<BasketItemMessage> Items { get; set; } = new();
}

public class BasketItemMessage
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}