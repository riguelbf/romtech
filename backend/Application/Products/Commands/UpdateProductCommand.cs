namespace Application.Products.Commands;

public record UpdateProductCommand : ICommand<int>
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
}