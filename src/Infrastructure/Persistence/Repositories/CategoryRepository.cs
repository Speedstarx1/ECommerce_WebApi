using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _appDbContext;

        public CategoryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _appDbContext.Categories.Add(category);
            await _appDbContext.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _appDbContext.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _appDbContext.Categories
                .Where(c => c.Id == id && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Category?> UpdateAsync(Category category)
        {
            var existingCategory = await GetByIdAsync(category.Id);
            if (existingCategory == null)
                return null;
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.UpdatedDate = DateTime.UtcNow;
            await _appDbContext.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await GetByIdAsync(id);
            if (category == null)
                return false;
            category.IsDeleted = true;
            category.UpdatedDate = DateTime.UtcNow;
            await _appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AlreadyExistsAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _appDbContext.Categories
                .Where(c => !c.IsDeleted)
                .AnyAsync(predicate);
        }

    }
}
