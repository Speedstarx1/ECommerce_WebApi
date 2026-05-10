using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository reviewRepository,
            IProductRepository productRepository,
            IAuthService authService,
            IMapper mapper,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
        }

        private Guid GetCustomerId()
        {
            var userId = _authService.GetSignedInUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("Customer not logged in.");
            return Guid.Parse(userId);
        }

        public async Task<ReviewDto> CreateAsync(ReviewRequestDto request)
        {
            var customerId = GetCustomerId();
            _logger.LogInformation("Creating review for product: {ProductId}", request.ProductId);

            // Check product exists
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID '{request.ProductId}' not found.");

            // Check customer has purchased the product
            var hasPurchased = await _reviewRepository.HasPurchasedProductAsync(customerId, request.ProductId);
            if (!hasPurchased)
                throw new InvalidOperationException("You can only review products you have purchased.");

            // Check customer hasn't already reviewed this product
            var existingReview = await _reviewRepository.GetByCustomerAndProductAsync(customerId, request.ProductId);
            if (existingReview != null)
                throw new InvalidOperationException("You have already reviewed this product.");

            var review = new Review(
                customerId: customerId,
                productId: request.ProductId,
                rating: request.Rating,
                comment: request.Comment
            );

            review.CreatedBy = _authService.GetSignedInEmail() ?? "System";

            var created = await _reviewRepository.CreateAsync(review);
            _logger.LogInformation("Review created with ID: {ReviewId}", created.Id);

            // Reload with includes for mapping
            var reviewWithIncludes = await _reviewRepository.GetByIdAsync(created.Id);
            return _mapper.Map<ReviewDto>(reviewWithIncludes!);
        }

        public async Task<List<ReviewDto>> GetByProductIdAsync(Guid productId)
        {
            _logger.LogInformation("Fetching reviews for product: {ProductId}", productId);
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> UpdateAsync(Guid reviewId, ReviewRequestDto request)
        {
            var customerId = GetCustomerId();
            _logger.LogInformation("Updating review: {ReviewId}", reviewId);

            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                return null;

            // Only the customer who created the review can update it
            if (review.CustomerId != customerId)
                throw new UnauthorizedAccessException("You can only update your own reviews.");

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.UpdatedDate = DateTime.UtcNow;
            review.UpdatedBy = _authService.GetSignedInEmail() ?? "System";

            var updated = await _reviewRepository.UpdateAsync(review);
            return _mapper.Map<ReviewDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid reviewId)
        {
            var customerId = GetCustomerId();
            _logger.LogInformation("Deleting review: {ReviewId}", reviewId);

            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null) return false;

            // Customer can delete their own review, admin can delete any
            if (review.CustomerId != customerId && !_authService.IsAdmin())
                throw new UnauthorizedAccessException("You can only delete your own reviews.");

            return await _reviewRepository.DeleteAsync(reviewId);
        }
    }
}