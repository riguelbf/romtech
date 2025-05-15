using SharedKernel;

namespace Domain.Products
{
    public class Product : EntityBase
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required decimal Price { get; set; }
        public required int Stock { get; set; }
    }
}
