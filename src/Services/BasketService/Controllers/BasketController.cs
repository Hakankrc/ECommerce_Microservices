using BasketService.Entities;
using BasketService.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Events;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace BasketService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public BasketController(IBasketRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
        var basket = await _repository.GetBasket(userName);
        // Return existing basket or a new one to prevent null reference
        return Ok(basket ?? new ShoppingCart(userName));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
    {
        // Update or create basket in Redis
        return Ok(await _repository.UpdateBasket(basket));
    }

    [HttpDelete("{userName}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteBasket(string userName)
    {
        await _repository.DeleteBasket(userName);
        return Ok();
    }

    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckoutEvent basketCheckout)
    {
        // 1. Get existing basket
        var basket = await _repository.GetBasket(basketCheckout.UserName);
        if (basket == null)
        {
            return BadRequest("Basket not found");
        }

        // 2. Set total price from basket data
        basketCheckout.TotalPrice = basket.TotalPrice;

        // 3. Publish checkout event to RabbitMQ (Async)
        await _publishEndpoint.Publish(basketCheckout);

        // 4. Remove the basket
        await _repository.DeleteBasket(basket.UserName);

        return Accepted();
    }
}