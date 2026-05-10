namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();

        public Category(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        protected Category() { }
    }
}