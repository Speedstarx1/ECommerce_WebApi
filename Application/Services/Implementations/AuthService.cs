
using Application.Dtos.RequestDto;
using Application.Dtos.ResponseDto;
using Application.Exceptions;
using Application.Helpers;
using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Security.Claims;

namespace Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        
        private readonly IUserRepository _userRepository;
        
        private readonly ICustomerRepository _customerRepository;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AuthService(
            
            IUserRepository userRepository,
            
            ICustomerRepository customerRepository,
            IJwtService jwtService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger)
        {
            
            
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _jwtService = jwtService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
                throw new ValidationException("Invalid email or password.");
            }

            // Verify password
            if (!UserHelper.VerifyPassword(request.Password, user.PasswordHash, user.HashSalt))
            {
                _logger.LogWarning("Login failed - invalid password for: {Email}", request.Email);
                throw new ValidationException("Invalid email or password.");
            }


            // Generate JWT token
            var token = _jwtService.GenerateToken(user);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");

            _logger.LogInformation("Login successful for: {Email}", request.Email);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserType = user.UserType.ToString(),
                    ProfilePictureUrl = user.ProfilePictureUrl
                }
            };
        }

        public async Task<AuthResponseDto> RegisterCustomerAsync(CustomerCreateDto request)
        {
            _logger.LogInformation("Registering new Customer with email: {Email}", request.Email);

            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ValidationException("A user with this email already exists.");
            }

            

            // Create password hash
            var (hash, salt) = UserHelper.GeneratePasswordHash(request.Password);

            var picture = "";
            /*if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
            {
                using var stream = new MemoryStream();
                await request.ProfilePicture.CopyToAsync(stream);

                byte[] profilePictureBytes = stream.ToArray();
                picture = Convert.ToBase64String(profilePictureBytes);
            }*/

            /* if (request.ProfilePicture != null && request.ProfilePicture.Length > 0)
             {
                 var fileName = request.ProfilePicture.ContentType.Split('/')[0];
                 string contentType = request.ProfilePicture.ContentType.Split('/')[1];
                 var name = $"{fileName}_{request.Email}_{Guid.NewGuid()}.{contentType}";
                 picture = await _fileService.UploadFile(request.ProfilePicture, name);
             }
            */
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
                createdBy: "Self-Registration",
                createdDate: DateTime.UtcNow,
                profilePictureUrl: picture
            );

            

            var createdCustomer = await _customerRepository.CreateAsync(customer);

           

            // Generate JWT token
            var token = _jwtService.GenerateToken(createdCustomer);
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");

            _logger.LogInformation("Customer registered successfully with ID: {CustomerId}, CustomerEmail: {Email}",
                createdCustomer.Id, createdCustomer.Email);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                User = new UserInfoDto
                {
                    Id = createdCustomer.Id,
                    Email = createdCustomer.Email,
                    FirstName = createdCustomer.FirstName,
                    LastName = createdCustomer.LastName,
                    UserType = createdCustomer.UserType.ToString(),
                    ProfilePictureUrl = picture
                }
            };
        }

        public string GetSignedInEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? "";
        }

        public bool IsCustomer()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("userType")?.Value == UserType.Customer.ToString();
        }
    }
}