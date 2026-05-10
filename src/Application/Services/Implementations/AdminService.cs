using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Exceptions;
using Application.Helpers;
using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.Internal;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Collections;

namespace Application.Services.Implementations
{
    public class AdminService : IAdminService
    {
        
        private readonly IAdminRepository _adminRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;
        private readonly IAuthService _authService;

        public AdminService(
            
            IAdminRepository adminRepository,
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<AdminService> logger,
            IAuthService authService)
        {
            
            _adminRepository = adminRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _authService = authService;
        }

        public async Task<AdminDto> CreateAsync(AdminCreateDto request)
        {
            _logger.LogInformation("Creating new admin with email: {Email}",
                request.Email);

            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ValidationException("A user with this email already exists.");
            }

            

            // Create password hash
            var (hash, salt) = UserHelper.GeneratePasswordHash(request.Password);

           

            var admin = new Admin(
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                passwordHash: hash,
                hashSalt: salt
            );

            admin.CreatedBy = _authService.GetSignedInEmail() ?? "System";

            var createdAdmin = await _adminRepository.CreateAsync(admin);

            _logger.LogInformation("Admin created successfully with ID: {AdminId}, AdminEmail: {Email}",
                createdAdmin.Id, createdAdmin.Email);

            return MapToDto(createdAdmin);
        }

        public async Task<AdminDto?> GetByIdAsync(Guid id)
        {
            var admin = await _adminRepository.GetByIdAsync(id);
            return admin == null ? null : MapToDto(admin);
        }

        public async Task<AdminDto?> GetByEmailAsync(string email)
        {
            var admin = await _adminRepository.GetByEmailAsync(email);
            return admin == null ? null : MapToDto(admin);
        }

        public async Task<AdminDto?> GetByReferenceAsync(string reference)
        {
            var admin = await _adminRepository.GetByRefNumberAsync(reference);
            return admin == null ? null : MapToDto(admin);
        }

        public async Task<List<AdminDto>> GetAllAsync()
        {
            var adminList = await _adminRepository.GetAllAsync();
            return adminList.Select(MapToDto).ToList();
        }

        



        private static AdminDto MapToDto(Admin admin)
        {
            return new AdminDto
            {
                Id = admin.Id,
                RefNumber = admin.RefNumber,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                CreatedDate = admin.CreatedDate,
                CreatedBy = admin.CreatedBy,
                UpdatedDate = admin.UpdatedDate,
                UpdatedBy = admin.UpdatedBy
            };
        }
    }
}