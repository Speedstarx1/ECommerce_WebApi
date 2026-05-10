using Domain.Enums;
using FluentValidation;

namespace Application.Dtos.RequestDto
{
    public class AdminUpdateRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }

    }

    public class AdminUpdateRequestValidator : AbstractValidator<AdminUpdateRequest>
    {
        public AdminUpdateRequestValidator()
        {
            RuleFor(s => s.Email)
                .EmailAddress()
                .When(s => !string.IsNullOrEmpty(s.Email))
                .WithMessage("Use a valid email address");

            RuleFor(s => s.FirstName)
                .MinimumLength(2)
                .When(s => !string.IsNullOrEmpty(s.FirstName))
                .WithMessage("First name must be at least 2 characters");

            RuleFor(s => s.LastName)
                .MinimumLength(2)
                .When(s => !string.IsNullOrEmpty(s.LastName))
                .WithMessage("Last name must be at least 2 characters");
        }
    }
}