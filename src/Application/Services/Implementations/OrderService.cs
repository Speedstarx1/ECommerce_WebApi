using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPaystackService _paystackService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IPaystackService paystackService,
            IAuthService authService,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _paystackService = paystackService;
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CheckoutResponseDto> CheckoutAsync(CheckoutRequestDto request)
        {
            var userId = Guid.Parse(_authService.GetSignedInUserId()!);
            var userEmail = _authService.GetSignedInEmail()!;

            _logger.LogInformation("Checkout initiated for user: {UserId}", userId);

            // Get user's cart
            var cart = await _cartRepository.GetByCustomerIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Your cart is empty.");

            // Validate all items still available
            foreach (var item in cart.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null || !product.IsAvailable)
                    throw new InvalidOperationException($"Product '{item.Product.Name}' is no longer available.");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Only {product.StockQuantity} units of '{product.Name}' available.");
            }

            // Calculate total
            var totalAmount = cart.Items.Sum(i => i.UnitPrice * i.Quantity);

            // Create order
            var order = new Order(userId, request.ShippingAddress, totalAmount);

            // Add order items
            foreach (var cartItem in cart.Items)
            {
                order.Items.Add(new OrderItem(
                    orderId: order.Id,
                    productId: cartItem.ProductId,
                    productName: cartItem.Product.Name,
                    quantity: cartItem.Quantity,
                    unitPrice: cartItem.UnitPrice
                ));
            }

            order.CreatedBy = userEmail;
            order = await _orderRepository.CreateAsync(order);

            // Initialize Paystack transaction
            var paystackResponse = await _paystackService.InitializeTransactionAsync(
                email: userEmail,
                amountNaira: totalAmount,
                reference: order.OrderNumber,
                callbackUrl: request.CallbackUrl
            );

            // Save Paystack reference to order
            order.PaystackReference = paystackResponse.Reference;
            order = await _orderRepository.UpdateAsync(order);

            _logger.LogInformation("Order {OrderNumber} created for customer {CustomerId}", order.OrderNumber, userId);

            return new CheckoutResponseDto
            {
                Order = _mapper.Map<OrderDto>(order),
                AuthorizationUrl = paystackResponse.AuthorizationUrl,
                PaystackReference = paystackResponse.Reference
            };
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto?> GetByOrderNumberAsync(string orderNumber)
        {
            var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<PagedResult<OrderDto>> GetMyOrdersAsync(int page, int pageSize)
        {
            var customerId = Guid.Parse(_authService.GetSignedInUserId()!);
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var paged = await _orderRepository.GetByCustomerIdAsync(customerId, page, pageSize);
            var dtos = _mapper.Map<List<OrderDto>>(paged.Items);
            return new PagedResult<OrderDto>(dtos, page, pageSize, paged.TotalCount);
        }

        public async Task<PagedResult<OrderDto>> GetAllOrdersAsync(int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var paged = await _orderRepository.GetAllAsync(page, pageSize);
            var dtos = _mapper.Map<List<OrderDto>>(paged.Items);
            return new PagedResult<OrderDto>(dtos, page, pageSize, paged.TotalCount);
        }

        public async Task<OrderDto?> UpdateStatusAsync(Guid id, OrderStatus status)
        {
            _logger.LogInformation("Updating order status: {OrderId} to {Status}", id, status);

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

            // Admin can only move forward through statuses, not backwards
            // and can't manually set Confirmed (Paystack does that)
            var allowedTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
            {
                { OrderStatus.Confirmed, new List<OrderStatus> { OrderStatus.Processing } },
                { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped } },
                { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
            };

            if (!allowedTransitions.ContainsKey(order.Status) ||
                !allowedTransitions[order.Status].Contains(status))
            {
                throw new InvalidOperationException(
                    $"Cannot transition order from {order.Status} to {status}.");
            }

            order.Status = status;
            order.UpdatedDate = DateTime.UtcNow;
            order.UpdatedBy = _authService.GetSignedInEmail() ?? "System";

            var updated = await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(updated);
        }

        public async Task HandleWebhookAsync(string rawBody, string signature)
        {
            // Verify the webhook is actually from Paystack
            if (!_paystackService.VerifyWebhookSignature(rawBody, signature))
            {
                _logger.LogWarning("Invalid Paystack webhook signature");
                throw new UnauthorizedAccessException("Invalid webhook signature.");
            }

            using var doc = JsonDocument.Parse(rawBody);
            var root = doc.RootElement;

            var eventType = root.TryGetProperty("event", out var ev) ? ev.GetString() : null;

            // Only handle successful payments
            if (eventType != "charge.success")
            {
                _logger.LogInformation("Paystack webhook event {Event} - no action taken", eventType);
                return;
            }

            var data = root.GetProperty("data");
            var reference = data.GetProperty("reference").GetString()!;
            var status = data.GetProperty("status").GetString() ?? "";

            _logger.LogInformation("Processing charge.success for ref: {Reference}", reference);

            var order = await _orderRepository.GetByPaystackReferenceAsync(reference);
            if (order == null)
            {
                _logger.LogWarning("No order found for Paystack reference: {Reference}", reference);
                return;
            }

            // Avoid processing twice
            if (order.Status == OrderStatus.Confirmed)
            {
                _logger.LogInformation("Order {OrderNumber} already confirmed - skipping", order.OrderNumber);
                return;
            }

            if (status == "success")
            {
                // Update order status
                order.Status = OrderStatus.Confirmed;
                order.PaidAt = DateTime.UtcNow;
                order.UpdatedDate = DateTime.UtcNow;
                order.UpdatedBy = "Paystack";

                // Reduce stock for each product
                foreach (var item in order.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;
                        product.IsAvailable = product.StockQuantity > 0;
                        await _productRepository.UpdateAsync(product);
                    }
                }

                // Clear the customer's cart
                var cart = await _cartRepository.GetByCustomerIdAsync(order.CustomerId);
                if (cart != null)
                    await _cartRepository.ClearCartAsync(cart.Id);

                await _orderRepository.UpdateAsync(order);

                _logger.LogInformation("Order {OrderNumber} confirmed and cart cleared", order.OrderNumber);
            }
            else
            {
                order.Status = OrderStatus.Cancelled;
                order.UpdatedDate = DateTime.UtcNow;
                order.UpdatedBy = "Paystack";
                await _orderRepository.UpdateAsync(order);

                _logger.LogInformation("Order {OrderNumber} cancelled due to failed payment", order.OrderNumber);
            }
        }
    }
}