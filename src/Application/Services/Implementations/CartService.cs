using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IAuthService authService,
            IMapper mapper,
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<CartDto> GetCartAsync()
        {
            var customerId = GetCustomerId();
            _logger.LogInformation("Fetching cart for customer: {CustomerId}", customerId);
               
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);

            // Auto-create cart if it doesn't exist
            if (cart == null)
            {
                cart = new Cart(customerId);
                cart = await _cartRepository.CreateAsync(cart);
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> AddItemAsync(CartItemRequestDto request)
        {
            var customerId = GetCustomerId();
            _logger.LogInformation("Adding item to cart for customer: {CustomerId}", customerId);

            // Check product exists and is available
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID '{request.ProductId}' not found.");

            if (!product.IsAvailable)
                throw new InvalidOperationException($"Product '{product.Name}' is not available.");

            if (product.StockQuantity < request.Quantity)
                throw new InvalidOperationException($"Only {product.StockQuantity} units of '{product.Name}' available.");

            // Get or create cart
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null)
            {
                cart = new Cart(customerId);
                cart = await _cartRepository.CreateAsync(cart);
            }

            // Check if product already in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existingItem != null)
            {
                // Update quantity instead of adding duplicate
                existingItem.Quantity += request.Quantity;
                existingItem.UpdatedDate = DateTime.UtcNow;
            }
            else
            {
                var cartItem = new CartItem(
                    cartId: cart.Id,
                    productId: request.ProductId,
                    quantity: request.Quantity,
                    unitPrice: product.Price
                );
                await _cartRepository.AddItemAsync(cartItem);
            }

            cart.UpdatedDate = DateTime.UtcNow;
            cart = await _cartRepository.UpdateAsync(cart);

            _logger.LogInformation("Item added to cart for customer: {CustomerId}", customerId);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> UpdateItemAsync(Guid cartItemId, CartItemRequestDto request)
        {
            var customerId = GetCustomerId();
            _logger.LogInformation("Updating cart item: {CartItemId} for customer: {CustomerId}", cartItemId, customerId);

            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found.");

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                throw new InvalidOperationException("Cart item not found.");

            // Check stock
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product!.StockQuantity < request.Quantity)
                throw new InvalidOperationException($"Only {product.StockQuantity} units of '{product.Name}' available.");

            item.Quantity = request.Quantity;
            item.UpdatedDate = DateTime.UtcNow;
            cart.UpdatedDate = DateTime.UtcNow;

            cart = await _cartRepository.UpdateAsync(cart);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> RemoveItemAsync(Guid cartItemId)
        {
            var customerId = GetCustomerId();
            _logger.LogInformation("Removing cart item: {CartItemId} for customer: {CustomerId}", cartItemId, customerId);

            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null)
                throw new InvalidOperationException("Cart not found.");

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                throw new InvalidOperationException("Cart item not found.");

            cart.Items.Remove(item);
            cart.UpdatedDate = DateTime.UtcNow;

            cart = await _cartRepository.UpdateAsync(cart);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> ClearCartAsync()
        {
            var customerId = GetCustomerId();
            _logger.LogInformation("Clearing cart for customer: {CustomerId}", customerId);

            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null) return false;

            return await _cartRepository.ClearCartAsync(cart.Id);
        }

        public async Task<CartDto> MergeCartAsync(MergeCartRequestDto request)
        {
            var customerId = GetCustomerId();
            _logger.LogInformation("Merging guest cart for customer: {CustomerId}", customerId);

            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null)
            {
                cart = new Cart(customerId);
                cart = await _cartRepository.CreateAsync(cart);
            }

            foreach (var guestItem in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(guestItem.ProductId);
                if (product == null || !product.IsAvailable) continue;

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == guestItem.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += guestItem.Quantity;
                    existingItem.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    cart.Items.Add(new CartItem(
                        cartId: cart.Id,
                        productId: guestItem.ProductId,
                        quantity: guestItem.Quantity,
                        unitPrice: product.Price
                    ));
                }
            }

            cart.UpdatedDate = DateTime.UtcNow;
            cart = await _cartRepository.UpdateAsync(cart);

            return _mapper.Map<CartDto>(cart);
        }

        private Guid GetCustomerId()
        {
            var userId = _authService.GetSignedInUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("Customer not logged in.");
            return Guid.Parse(userId);
        }
    }
}