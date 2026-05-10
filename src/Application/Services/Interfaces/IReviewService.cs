using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;

namespace Application.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateAsync(ReviewRequestDto request);
        Task<List<ReviewDto>> GetByProductIdAsync(Guid productId);
        Task<ReviewDto?> UpdateAsync(Guid reviewId, ReviewRequestDto request);
        Task<bool> DeleteAsync(Guid reviewId);
    }
}