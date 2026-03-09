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
        // Return existing basket or a new one to prevent null references
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
    var basket = await _repository.GetBasket(basketCheckout.UserName);
    if (basket == null)
    {
        return BadRequest("Basket not found");
    }

    basketCheckout.TotalPrice = basket.TotalPrice;
    
    basketCheckout.Items = basket.Items.Select(item => new BasketItemMessage 
    { 
        ProductId = int.Parse(item.ProductId),
        Quantity = item.Quantity 
    }).ToList();

    await _publishEndpoint.Publish(basketCheckout);

    await _repository.DeleteBasket(basket.UserName);

    return Accepted();
}
}