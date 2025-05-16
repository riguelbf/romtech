namespace Application.Products.Commands;

public record CreateProductCommand : ICommand<int>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required decimal Price { get; init; }
    public required int Stock { get; init; }
}
