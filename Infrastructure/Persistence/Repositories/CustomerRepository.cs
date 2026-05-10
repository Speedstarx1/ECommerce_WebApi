using Application.Dtos.Common;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _appDbContext;

        public CustomerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            _appDbContext.Customers.Add(customer);
            await _appDbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _appDbContext.Customers
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Customer?> GetAsync(string refNumber)
        {
            return await _appDbContext.Customers
                .Where(c => c.RefNumber == refNumber && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _appDbContext.Customers
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _appDbContext.Customers
                .Where(c => c.Email == email && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        

        public async Task<PagedResult<Customer>> SearchAsync(string? searchTerm, int page, int pageSize, string? sortBy)
        {
            var query = _appDbContext.Customers.Where(c => !c.IsDeleted).AsQueryable();

            
            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(c =>
                    c.FirstName.ToLower().Contains(term) ||
                    c.LastName.ToLower().Contains(term) ||
                    c.Email.ToLower().Contains(term) ||
                    c.RefNumber.ToLower().Contains(term));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "firstname" => query.OrderBy(c => c.FirstName),
                "firstname_desc" => query.OrderByDescending(c => c.FirstName),
                "lastname" => query.OrderBy(c => c.LastName),
                "lastname_desc" => query.OrderByDescending(c => c.LastName),
                "matricnumber" => query.OrderBy(c => c.RefNumber),
                "matricnumber_desc" => query.OrderByDescending(c => c.RefNumber),
                "createddate" => query.OrderBy(c => c.CreatedDate),
                "createddate_desc" => query.OrderByDescending(c => c.CreatedDate),
                _ => query.OrderByDescending(c => c.CreatedDate)
            };

            // Apply pagination
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Customer>(items, page, pageSize, totalCount);
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            customer.UpdatedDate = DateTime.UtcNow;
            _appDbContext.Customers.Update(customer);
            await _appDbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var customer = await GetByIdAsync(id);
            if (customer == null)
                return false;

            // Soft delete
            customer.IsDeleted = true;
            customer.UpdatedDate = DateTime.UtcNow;
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _appDbContext.Customers.AnyAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}