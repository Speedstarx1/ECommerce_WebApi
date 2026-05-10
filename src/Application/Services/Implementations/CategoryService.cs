using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> CreateAsync(CategoryRequestDto request)
        {
            var alreadyexists = await _categoryRepository.AlreadyExistsAsync(c => c.Name == request.Name);
            if (alreadyexists)
            {
                // Handle the case when the category already exists
                throw new InvalidOperationException("Category with the same name already exists.");
            }

            var category = new Category(request.Name, request.Description);
            category = await _categoryRepository.CreateAsync(category);

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return categoryDto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            return await _categoryRepository.DeleteAsync(id);
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => _mapper.Map<CategoryDto>(c)).ToList();

        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id); 
            if (category == null)
                return null;

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto?> UpdateAsync(Guid id, CategoryRequestDto request)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            if (!string.IsNullOrWhiteSpace(request.Name))
                category.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Description))
                category.Description = request.Description;

            category.UpdatedDate = DateTime.UtcNow;

            var updated = await _categoryRepository.UpdateAsync(category);
            return _mapper.Map<CategoryDto>(updated);
        }
    }
}





