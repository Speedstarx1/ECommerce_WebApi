using Application.Dtos.Common;
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Exceptions;
using Application.Helpers;
using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IAuthService _authService;
        
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            IAuthService authService,
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILogger<CustomerService> logger)
        {
            _authService = authService;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerDto> CreateAsync(CustomerCreateDto request)
        {
            _logger.LogInformation("Creating new student with email: {Email}", request.Email);

            

            // Create password hash (in production, use proper hashing)
            var (hash, salt) = UserHelper.GeneratePasswordHash(request.Password);

            var customer = new Customer(
                
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                phoneNumber: request.PhoneNo,
                passwordHash: hash,
                hashSalt: salt,
                gender: request.Gender,
                userType: UserType.Customer,
                address: request.Address,
                createdBy: request.Email,
                createdDate: DateTime.UtcNow
            );

            

            var createdCustomer= await _customerRepository.CreateAsync(customer);
            _logger.LogInformation("Customer created successfully with ID: {CustomerId}, Email: {CustomerEmail}",
                createdCustomer.Id, createdCustomer.Email);

            return _mapper.Map<CustomerDto>(createdCustomer);
        }

        public async Task<CustomerDto?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching customer by ID: {CustomerId}", id);
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                _logger.LogWarning("Customer with ID: {CustomerId} not found", id);
                return null;
            }

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto?> GetAsync(string refNumber)
        {
            _logger.LogInformation("Fetching customer by RefNumber: {RefNumber}", refNumber);

            var customer = await _customerRepository.GetAsync(refNumber);

            if (customer == null)
            {
                _logger.LogWarning("Customer with RefNumber: {RefNumber} not found", refNumber);
                throw new EntityNotFoundException($"Customer with RefNumber '{refNumber}' not found.");
            }

            var signedInEmail = _authService.GetSignedInEmail();
            if (_authService.IsCustomer() && signedInEmail != customer.Email)
            {
                _logger.LogWarning("Unauthorized access attempt to get customer details with RefNumber: {RefNumber} by email: {Email}", refNumber, signedInEmail);
                throw new UnauthorizedAccessException("You are not authorized to access this customer's information.");
            }

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all customers");
            var customers = await _customerRepository.GetAllAsync();
            _logger.LogInformation("Found {Count} customers", customers.Count);
            return _mapper.Map<List<CustomerDto>>(customers);
        }

        public async Task<PagedResult<CustomerDto>> SearchAsync(string? searchTerm, int page, int pageSize, string? sortBy)
        {
            _logger.LogInformation(
                "Searching customers - SearchTerm: {SearchTerm}, Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}",
                searchTerm, page, pageSize, sortBy);

            // Ensure valid pagination parameters
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var pagedCustomers = await _customerRepository.SearchAsync(searchTerm, page, pageSize, sortBy);

            _logger.LogInformation("Search returned {Count} customers out of {Total} total",
                pagedCustomers.Items.Count, pagedCustomers.TotalCount);

            var customerDtos = _mapper.Map<List<CustomerDto>>(pagedCustomers.Items);

            return new PagedResult<CustomerDto>(customerDtos, page, pageSize, pagedCustomers.TotalCount);
        }

        public async Task<CustomerDto?> UpdateAsync(Guid id, CustomerUpdateRequest updateRequest)
        {
            _logger.LogInformation("Updating customer with ID: {CustomerId}", id);

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Cannot update - Customer with ID: {CustomerId} not found", id);
                return null;
            }

            // Apply updates only for non-null values
            if (!string.IsNullOrEmpty(updateRequest.FirstName))
                customer.FirstName = updateRequest.FirstName;

            if (!string.IsNullOrEmpty(updateRequest.LastName))
                customer.LastName = updateRequest.LastName;

            if (!string.IsNullOrEmpty(updateRequest.Email))
                customer.Email = updateRequest.Email;

            if (!string.IsNullOrEmpty(updateRequest.PhoneNumber))
                customer.PhoneNumber = updateRequest.PhoneNumber;

            if (!string.IsNullOrEmpty(updateRequest.Address))
                customer.Address = updateRequest.Address;

            

            customer.UpdatedBy = "System";
            customer.UpdatedDate = DateTime.UtcNow;

            var updatedCustomer = await _customerRepository.UpdateAsync(customer);
            _logger.LogInformation("Customer with ID: {CustomerId} updated successfully", id);

            return _mapper.Map<CustomerDto>(updatedCustomer);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting customer with ID: {CustomerId}", id);

            var result = await _customerRepository.DeleteAsync(id);

            if (result)
                _logger.LogInformation("Customer with ID: {CustomerId} deleted successfully", id);
            else
                _logger.LogWarning("Cannot delete - Customer with ID: {CustomerId} not found", id);

            return result;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _customerRepository.ExistsAsync(id);
        }

        

    }
}