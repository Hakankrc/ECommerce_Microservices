using MediatR;
using MassTransit;
using ProductService.Domain;
using ProductService.Infrastructure;
using Shared.Events;

namespace ProductService.Application.Features.Products.Commands;

// Command: Represents the request to create a new product
public record CreateProductCommand(string Name, decimal Price, string Description, int Stock, string PictureUrl) : IRequest<int>;

// Handler: Contains the business logic for the CreateProductCommand
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
        // 1. Persist data to the local database
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

        // 2. Publish Integration Event to RabbitMQ:
        // Notifies other microservices (like Basket or Stock) that a new product is created
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