using FluentValidation;

namespace Application.Dtos.RequestDto
{
    public class CartItemRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemRequestValidator : AbstractValidator<CartItemRequestDto>
    {
        public CartItemRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.")
                .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100.");
        }
    }
}