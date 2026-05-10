namespace Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public Review(Guid customerId, Guid productId, int rating, string? comment)
        {
            CustomerId = customerId;
            ProductId = productId;
            Rating = rating;
            Comment = comment;
        }

        protected Review() { }
    }
}