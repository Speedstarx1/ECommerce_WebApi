using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Dtos.RequestDto
{
    public class CustomerUpdateRequest
    {
       
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; } 
        public Gender? Gender { get; set; }
        
        
    }

    public class CustomerUpdateRequestValidator : AbstractValidator<CustomerUpdateRequest>
    {
        public CustomerUpdateRequestValidator()
        {
            RuleFor(s => s.Email)
                .EmailAddress().WithMessage("Use a valid email address")
                .When(s => !string.IsNullOrWhiteSpace(s.Email));

            RuleFor(s => s.FirstName)
                .MinimumLength(2).WithMessage("First name must be at least 2 characters")
                .When(s => !string.IsNullOrWhiteSpace(s.FirstName));

            RuleFor(s => s.LastName)
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
                .When(s => !string.IsNullOrWhiteSpace(s.LastName));

            RuleFor(x => x.Password)
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must be Alphanumeric.")
                .When(x => !string.IsNullOrWhiteSpace(x.Password));

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.")
                .When(x => !string.IsNullOrWhiteSpace(x.Password));
        }
    }
}