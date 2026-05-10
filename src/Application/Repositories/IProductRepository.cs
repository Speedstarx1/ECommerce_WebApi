using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Common;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Repositories
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<Product?> GetByIdAsync(Guid id);
        Task<PagedResult<Product>> SearchAsync(string? searchTerm, Guid? categoryId, int page, int pageSize, string? sortBy);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Expression<Func<Product, bool>> predicate);
    }
}