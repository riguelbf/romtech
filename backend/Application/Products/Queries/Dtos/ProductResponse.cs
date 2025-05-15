namespace Application.Products.Queries.Dtos;

public record ProductResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
}