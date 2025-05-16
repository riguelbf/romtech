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
        
        /// <summary>
        /// Used for optimistic concurrency control.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Timestamp]
        public byte[]? RowVersion { get; set; }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");
            
            Stock += quantity;
        }
    }
}
