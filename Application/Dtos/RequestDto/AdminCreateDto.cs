using Domain.Enums;
using FluentValidation;

namespace Application.Dtos.RequestDto
{
    public class AdminCreateDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public Gender Gender { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;
       
    }

    public class AdminRequestValidator : AbstractValidator<AdminCreateDto>
    {
        public AdminRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MinimumLength(2)
                .When(s => !string.IsNullOrEmpty(s.FirstName))
                .WithMessage("Last name must be at least 2 characters"); 

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MinimumLength(2)
                .When(s => !string.IsNullOrEmpty(s.LastName))
                .WithMessage("Last name must be at least 2 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must be Aplhanumeric.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

        }
    }
}