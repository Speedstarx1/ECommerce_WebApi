using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Dtos.RequestDto
{
    public class MergeCartRequestDto
    {
        public List<CartItemRequestDto> Items { get; set; } = new();
    }

    public class MergeCartRequestValidator : AbstractValidator<MergeCartRequestDto>
    {
        public MergeCartRequestValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Cart items cannot be empty.");

            RuleForEach(x => x.Items).SetValidator(new CartItemRequestValidator());
        }
    }
}