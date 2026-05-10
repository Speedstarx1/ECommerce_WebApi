using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RegisterCustomerAsync(CustomerCreateDto request);
        string GetSignedInEmail();
        bool IsCustomer();
        string? GetSignedInUserId();
        bool IsAdmin();
    }
}
