using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Domain.Enums;

namespace Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<CheckoutResponseDto> CheckoutAsync(CheckoutRequestDto request);
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task<OrderDto?> GetByOrderNumberAsync(string orderNumber);
        Task<PagedResult<OrderDto>> GetMyOrdersAsync(int page, int pageSize);
        Task<PagedResult<OrderDto>> GetAllOrdersAsync(int page, int pageSize);
        Task HandleWebhookAsync(string rawBody, string signature);
        Task<OrderDto?> UpdateStatusAsync(Guid id, OrderStatus status);
    }
}