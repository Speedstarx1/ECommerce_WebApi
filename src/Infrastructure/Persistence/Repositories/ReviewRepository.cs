using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review> CreateAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<List<Review>> GetByProductIdAsync(Guid productId)
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<Review?> GetByCustomerAndProductAsync(Guid customerId, Guid productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r =>
                    r.CustomerId == customerId &&
                    r.ProductId == productId &&
                    !r.IsDeleted);
        }

        public async Task<bool> HasPurchasedProductAsync(Guid customerId, Guid productId)
        {
            return await _context.Orders
                .AnyAsync(o =>
                    o.CustomerId == customerId &&
                    o.Status != OrderStatus.Pending &&
                    o.Status != OrderStatus.Cancelled &&
                    !o.IsDeleted &&
                    o.Items.Any(i => i.ProductId == productId));
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return false;

            review.IsDeleted = true;
            review.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}