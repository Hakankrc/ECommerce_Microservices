namespace Shared.Events;

public interface IProductCreatedEvent
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string PictureUrl { get; set; }
}