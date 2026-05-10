using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;

namespace Application.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync();
        Task<CartDto> AddItemAsync(CartItemRequestDto request);
        Task<CartDto> UpdateItemAsync(Guid cartItemId, CartItemRequestDto request);
        Task<CartDto> RemoveItemAsync(Guid cartItemId);
        Task<bool> ClearCartAsync();
        Task<CartDto> MergeCartAsync(MergeCartRequestDto request);
    }
}