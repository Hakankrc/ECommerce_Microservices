using MassTransit;
using Shared.Events;
using ProductService.Domain;
using ProductService.Infrastructure;

namespace ProductService.Consumers;

public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
{
    private readonly ProductDbContext _context;

    public BasketCheckoutConsumer(ProductDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        foreach (var item in context.Message.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.Stock -= item.Quantity;
                if (product.Stock < 0) product.Stock = 0;
            }
        }

        await _context.SaveChangesAsync();
        Console.WriteLine("Stock levels successfully updated.");
    }
}