using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Infrastructure.Persistence.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Admin> CreateAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var admin = await GetByIdAsync(id);
            if (admin == null)
                return false;

            admin.IsDeleted = true;
            admin.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Admins.AnyAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<List<Admin>> GetAllAsync()
        {
            return await _context.Admins
                .Where(a => !a.IsDeleted)
                .ToListAsync();
        }

       

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _context.Admins
                .Where(a => a.Email == email && !a.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Admin?> GetByIdAsync(Guid id)
        {
            return await _context.Admins
                .Where(a => a.Id == id && !a.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Admin?> GetByRefNumberAsync(string refNo)
        {
            return await _context.Admins
                .Where(a => a.RefNumber == refNo && !a.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<Admin> UpdateAsync(Admin admin)
        {
            admin.UpdatedDate = DateTime.UtcNow;
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
            return admin;
        }
    }
}