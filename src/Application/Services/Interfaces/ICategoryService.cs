using Application.Dtos.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.ResponseDto;


namespace Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateAsync(CategoryRequestDto category);
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(Guid id);
        Task<CategoryDto?> UpdateAsync(Guid id, CategoryRequestDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}
