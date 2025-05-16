namespace Application.Products.Commands;

public record AddStockCommand : ICommand<int>
{
    public int Quantity { get; set; }
    
    public int? Id { get; set; }
}
