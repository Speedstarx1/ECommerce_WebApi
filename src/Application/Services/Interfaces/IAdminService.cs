using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDto> CreateAsync(AdminCreateDto request);
        Task<AdminDto?> GetByIdAsync(Guid id);
        Task<List<AdminDto>> GetAllAsync();
        Task<AdminDto?> GetByEmailAsync(string email);
        Task<AdminDto?> GetByReferenceAsync(string reference);

    }
}
