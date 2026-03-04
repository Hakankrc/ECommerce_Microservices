using BasketService.Entities;
using BasketService.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;

    public BasketController(IBasketRepository repository)
    {
        _repository = repository;
    }

    // GET api/basket/{userName}
    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
        var basket = await _repository.GetBasket(userName);
        
        // If basket is null, return a new empty basket to avoid client-side null checks
        return Ok(basket ?? new ShoppingCart(userName));
    }

    // POST api/basket
    [HttpPost]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
    {
        // Update or Create basket in Redis
        var updatedBasket = await _repository.UpdateBasket(basket);
        return Ok(updatedBasket);
    }

    // DELETE api/basket/{userName}
    [HttpDelete("{userName}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteBasket(string userName)
    {
        await _repository.DeleteBasket(userName);
        return Ok();
    }
}