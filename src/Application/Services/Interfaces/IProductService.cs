using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;

namespace Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> CreateAsync(ProductCreateDto request);
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<PagedResult<ProductDto>> SearchAsync(string? searchTerm, Guid? categoryId, int page, int pageSize, string? sortBy);
        Task<ProductDto?> UpdateAsync(Guid id, ProductUpdateDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}