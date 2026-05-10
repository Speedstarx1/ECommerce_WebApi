using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto> CreateAsync(CustomerCreateDto request);
        Task<CustomerDto?> GetByIdAsync(Guid id);
        Task<CustomerDto?> GetAsync(string refNumber);
        Task<CustomerDto?> GetByEmailAsync(string email);
        Task<List<CustomerDto>> GetAllAsync();
        Task<PagedResult<CustomerDto>> SearchAsync(string? searchTerm, int page, int pageSize, string? sortBy);
        Task<CustomerDto?> UpdateAsync(Guid id, CustomerUpdateRequest updateRequest);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
