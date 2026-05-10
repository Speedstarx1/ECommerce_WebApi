namespace Domain.Entities
{
    public class Cart : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        public Cart(Guid customerId)
        {
            CustomerId = customerId;
        }

        protected Cart() { }
    }
}