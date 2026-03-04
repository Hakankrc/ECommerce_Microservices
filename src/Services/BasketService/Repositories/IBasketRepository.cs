using BasketService.Entities;

namespace BasketService.Repositories;

/// <summary>
/// Defines the contract for Basket data access operations.
/// </summary>
public interface IBasketRepository
{
    Task<ShoppingCart?> GetBasket(string userName);
    Task<ShoppingCart?> UpdateBasket(ShoppingCart basket);
    Task DeleteBasket(string userName);
}