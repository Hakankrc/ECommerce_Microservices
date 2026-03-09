using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain;
using ProductService.Infrastructure;

namespace ProductService.Application.Features.Products.Queries;


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
        
        return await _context.Products.AsNoTracking().ToListAsync(cancellationToken);
    }
}