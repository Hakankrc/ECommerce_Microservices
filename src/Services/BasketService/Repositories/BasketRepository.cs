using BasketService.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace BasketService.Repositories;

/// <summary>
/// Implementation of IBasketRepository using Redis via IDistributedCache.
/// </summary>
public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _redisCache;

    public BasketRepository(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }

    public async Task<ShoppingCart?> GetBasket(string userName)
    {
        // Retrieve the basket as a JSON string from Redis
        var basket = await _redisCache.GetStringAsync(userName);

        if (String.IsNullOrEmpty(basket))
            return null;

        // Deserialize JSON string back to ShoppingCart object
        return JsonConvert.DeserializeObject<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart?> UpdateBasket(ShoppingCart basket)
    {
        // Serialize the object to JSON string because Redis stores strings/bytes
        var jsonBasket = JsonConvert.SerializeObject(basket);

        // Update or Create the basket in Redis
        await _redisCache.SetStringAsync(basket.UserName, jsonBasket);

        return await GetBasket(basket.UserName);
    }

    public async Task DeleteBasket(string userName)
    {
        await _redisCache.RemoveAsync(userName);
    }
}