using MediatR;
using MassTransit;
using ProductService.Domain;
using ProductService.Infrastructure;
using Shared.Events;

namespace ProductService.Application.Features.Products.Commands;


public class CreateProductCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty; // Varsayılan değer
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string PictureUrl { get; set; } = string.Empty;
}


public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly ProductDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateProductCommandHandler(ProductDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            Stock = request.Stock,
            PictureUrl = request.PictureUrl
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish<IProductCreatedEvent>(new
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            PictureUrl = product.PictureUrl
        }, cancellationToken);
        
        return product.Id;
    }
}