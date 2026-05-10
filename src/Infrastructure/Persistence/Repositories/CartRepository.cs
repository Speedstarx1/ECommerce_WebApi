using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .AsTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && !c.IsDeleted);
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateAsync(Cart cart)
        {
            // Handle new cart items that aren't tracked yet
            foreach (var item in cart.Items)
            {
                if (item.Id == Guid.Empty || _context.Entry(item).State == EntityState.Detached)
                {
                    _context.CartItems.Add(item);
                }
            }

            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartItem> AddItemAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null) return false;

            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}