using MediatR;
using ProductService.Domain;
using ProductService.Infrastructure;

namespace ProductService.Application.Features.Products.Commands;

// 1. İstek (Request) Sınıfı: Dışarıdan ne gelecek? Cevap olarak ne dönecek (int: ProductId)?
public record CreateProductCommand(string Name, decimal Price, string Description, int Stock, string PictureUrl) : IRequest<int>;

// 2. İşleyici (Handler) Sınıfı: İsteği alıp veritabanına işleyen kısım.
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly ProductDbContext _context;

    public CreateProductCommandHandler(ProductDbContext context)
    {
        _context = context;
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

        // İleride buraya RabbitMQ eventi ekleyeceğiz! (4. Gün)
        
        return product.Id;
    }
}