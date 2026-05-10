using Application.Dtos.Common;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order?> GetByOrderNumberAsync(string orderNumber);
        Task<Order?> GetByPaystackReferenceAsync(string reference);
        Task<PagedResult<Order>> GetByCustomerIdAsync(Guid customerId, int page, int pageSize);
        Task<PagedResult<Order>> GetAllAsync(int page, int pageSize);
        Task<Order> UpdateAsync(Order order);
    }
}