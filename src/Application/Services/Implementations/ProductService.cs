using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Repositories;
using Application.Services.Contracts;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAuthService _authService;
        private readonly IFileServiceFactory _fileServiceFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IAuthService authService,
            IFileServiceFactory fileServiceFactory,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _authService = authService;
            _fileServiceFactory = fileServiceFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto request)
        {
            _logger.LogInformation("Creating product: {Name}", request.Name);

            // Check category exists
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new InvalidOperationException($"Category with ID '{request.CategoryId}' not found.");

            // Handle image upload
            string? imageUrl = null;
            if (request.Image != null)
            {
                
                var fileService = _fileServiceFactory.Create();
                var fileName = $"ecommerce/products/{Guid.NewGuid()}_{request.Image.FileName}";
                imageUrl = await fileService.UploadFile(request.Image, fileName);
            }

            var product = new Product(
                name: request.Name,
                description: request.Description,
                price: request.Price,
                stockQuantity: request.StockQuantity,
                categoryId: request.CategoryId,
                imageUrl: imageUrl
            );

            product.CreatedBy = _authService.GetSignedInEmail() ?? "System";

            var created = await _productRepository.CreateAsync(product);
            _logger.LogInformation("Product created with ID: {ProductId}", created.Id);

            return _mapper.Map<ProductDto>(created);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching product by ID: {ProductId}", id);
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found", id);
                return null;
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<PagedResult<ProductDto>> SearchAsync(string? searchTerm, Guid? categoryId, int page, int pageSize, string? sortBy)
        {
            _logger.LogInformation("Searching products - SearchTerm: {SearchTerm}, CategoryId: {CategoryId}", searchTerm, categoryId);

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var pagedProducts = await _productRepository.SearchAsync(searchTerm, categoryId, page, pageSize, sortBy);

            var productDtos = _mapper.Map<List<ProductDto>>(pagedProducts.Items);
            return new PagedResult<ProductDto>(productDtos, page, pageSize, pagedProducts.TotalCount);
        }

        public async Task<ProductDto?> UpdateAsync(Guid id, ProductUpdateDto request)
        {
            _logger.LogInformation("Updating product with ID: {ProductId}", id);

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found", id);
                return null;
            }

            // Validate category if being changed
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                    throw new InvalidOperationException($"Category with ID '{request.CategoryId}' not found.");
                product.CategoryId = request.CategoryId.Value;
            }

            // Handle image upload
            if (request.Image != null)
            {
                var fileService = _fileServiceFactory.Create();
                var fileName = $"products/{id}_{request.Image.FileName}";
                product.ImageUrl = await fileService.UploadFile(request.Image, fileName);
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                product.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Description))
                product.Description = request.Description;

            if (request.Price.HasValue)
                product.Price = request.Price.Value;

            if (request.StockQuantity.HasValue)
            {
                product.StockQuantity = request.StockQuantity.Value;
                product.IsAvailable = request.StockQuantity.Value > 0;
            }

            product.UpdatedDate = DateTime.UtcNow;

            var updated = await _productRepository.UpdateAsync(product);
            _logger.LogInformation("Product with ID: {ProductId} updated successfully", id);

            return _mapper.Map<ProductDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", id);
            var result = await _productRepository.DeleteAsync(id);

            if (result)
                _logger.LogInformation("Product with ID: {ProductId} deleted successfully", id);
            else
                _logger.LogWarning("Product with ID: {ProductId} not found", id);

            return result;
        }
    }
}