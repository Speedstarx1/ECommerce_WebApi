using Domain.Enums;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public Product(string name, string description, decimal price, int stockQuantity, Guid categoryId, string? imageUrl = null)
        {
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            CategoryId = categoryId;
            ImageUrl = imageUrl;
            IsAvailable = stockQuantity > 0;
        }

        protected Product() { }
    }
}