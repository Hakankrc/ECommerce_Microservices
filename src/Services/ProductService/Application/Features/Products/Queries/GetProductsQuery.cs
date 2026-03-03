using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain;
using ProductService.Infrastructure;

namespace ProductService.Application.Features.Products.Queries;

// İstek: Parametre yok, hepsini getir. Cevap: List<Product>
public record GetProductsQuery : IRequest<List<Product>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<Product>>
{
    private readonly ProductDbContext _context;

    public GetProductsQueryHandler(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // AsNoTracking() okuma işlemini hızlandırır (Senior Taktiği)
        return await _context.Products.AsNoTracking().ToListAsync(cancellationToken);
    }
}