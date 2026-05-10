using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin> CreateAsync(Admin admin);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<Admin>> GetAllAsync();
        Task<Admin?> GetByEmailAsync(string email);
        Task<Admin?> GetByIdAsync(Guid id);
        Task<Admin?> GetByRefNumberAsync(string refNo);

    }
}
