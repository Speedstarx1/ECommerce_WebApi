using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Dtos.RequestDto
{
    public class AdminUpdateRequest
    {
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        
        public Gender? Gender { get; set; }

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

            RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender.")
            .When(x => x.Gender.HasValue);

            RuleFor(x => x.Password)
           .NotEmpty().WithMessage("Password is required.")
           .MinimumLength(8)
           .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
           .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
           .Matches("[0-9]").WithMessage("Password must be Alphanumeric.");
        }
    }
}