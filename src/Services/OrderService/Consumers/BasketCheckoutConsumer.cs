using MassTransit;
using OrderService.Domain;
using OrderService.Infrastructure;
using Shared.Events;


namespace OrderService.Consumers;

public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
{
    private readonly OrderDbContext _dbContext;
    private readonly ILogger<BasketCheckoutConsumer> _logger;

    public BasketCheckoutConsumer(OrderDbContext dbContext, ILogger<BasketCheckoutConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        // Map event to Order entity
        var order = new OrderService.Domain.Order
        {
            UserName = context.Message.UserName,
            TotalPrice = context.Message.TotalPrice,
            FirstName = context.Message.FirstName,
            LastName = context.Message.LastName,
            EmailAddress = context.Message.EmailAddress,
            AddressLine = context.Message.AddressLine,
            Country = context.Message.Country,
            CreatedDate = DateTime.UtcNow
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Order created successfully. Order ID: {OrderId}", order.Id);
    }
}