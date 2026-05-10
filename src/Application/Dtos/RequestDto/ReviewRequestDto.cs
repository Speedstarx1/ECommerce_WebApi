using FluentValidation;

namespace Application.Dtos.RequestDto
{
    public class ReviewRequestDto
    {
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }

    public class ReviewRequestValidator : AbstractValidator<ReviewRequestDto>
    {
        public ReviewRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Comment));
        }
    }
}