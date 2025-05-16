namespace Application.Products.Commands;

public record ReductionProductStockCommand : ICommand<int>
{
    public int Quantity { get; set; }
    
    public int? Id { get; set; }
}