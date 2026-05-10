using Domain.Enums;

namespace Application.Dtos.RequestDto
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}