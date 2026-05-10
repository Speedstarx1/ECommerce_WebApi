using FluentValidation;

namespace Application.Dtos.RequestDto
{
    public class CheckoutRequestDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string ShippingAddress { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string CallbackUrl { get; set; } = default!;
    }

    public class CheckoutRequestValidator : AbstractValidator<CheckoutRequestDto>
    {
        public CheckoutRequestValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required.")
                .MinimumLength(10).WithMessage("Please provide a full shipping address.");

            RuleFor(x => x.CallbackUrl)
                .NotEmpty().WithMessage("Callback URL is required.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Callback URL must be a valid URL.");
        }
    }
}