using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Features.Products.Commands;
using ProductService.Application.Features.Products.Queries;
using ProductService.Domain;


namespace ProductService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    
    public async Task<IActionResult> GetAll()
    {
        var products = await _mediator.Send(new GetProductsQuery());
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        // Validation logic is handled by ValidationBehavior pipeline.
        // We can safely send the command to the handler.
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = productId }, command);
    }
}