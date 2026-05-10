using Domain.Entities;

namespace Application.Repositories
{
    public interface IReviewRepository
    {
        Task<Review> CreateAsync(Review review);
        Task<Review?> GetByIdAsync(Guid id);
        Task<List<Review>> GetByProductIdAsync(Guid productId);
        Task<Review?> GetByCustomerAndProductAsync(Guid customerId, Guid productId);
        Task<bool> HasPurchasedProductAsync(Guid customerId, Guid productId);
        Task<Review> UpdateAsync(Review review);
        Task<bool> DeleteAsync(Guid id);
    }
}