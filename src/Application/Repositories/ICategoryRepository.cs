using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<Category?> UpdateAsync(Category category);
        Task<bool> AlreadyExistsAsync(Expression<Func<Category, bool>> predicate);
    }
}
