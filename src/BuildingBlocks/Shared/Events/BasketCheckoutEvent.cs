namespace Shared.Events;

/// <summary>
/// Bu sınıf, sepetten siparişe aktarılacak tüm bilgileri taşıyan "Zarf"tır.
/// </summary>
public class BasketCheckoutEvent
{
    public string UserName { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }

    // Müşteri Bilgileri
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}