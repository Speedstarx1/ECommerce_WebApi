using Application.Dtos.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateAsync(Customer customer);
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetAsync(string refNumber);
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<PagedResult<Customer>> SearchAsync(string? searchTerm, int page, int pageSize, string? sortBy);
        Task<Customer> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);


    }
}
