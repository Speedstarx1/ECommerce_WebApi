using Domain.Enums;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = default!;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = default!;
        public string? PaystackReference { get; set; }
        public DateTime? PaidAt { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public Order(Guid customerId, string shippingAddress, decimal totalAmount)
        {
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
            TotalAmount = totalAmount;
            OrderNumber = GenerateOrderNumber();
            Status = OrderStatus.Pending;
        }

        protected Order() { }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8).ToUpper()}";
        }
    }
}