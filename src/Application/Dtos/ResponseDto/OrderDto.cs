using Domain.Enums;

namespace Application.Dtos.ResponseDto
{

    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = default!;
        public Guid CustomerId { get; set; }
        public string Status { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = default!;
        public string? PaystackReference { get; set; }
        public DateTime? PaidAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    
}