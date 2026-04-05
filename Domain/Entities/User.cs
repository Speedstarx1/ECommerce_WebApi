using Domain.Enums;
using System.Reflection;

namespace Domain.Entities
{
    public abstract class User : BaseEntity
    {
       

        public string? ProfilePictureUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string HashSalt { get; set; }
        public UserType UserType { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }

        

        public User(string firstName, string lastName, string email, string phoneNumber, string passwordHash, string hashSalt, Gender gender, UserType userType, string address, string? profilePictureUrl = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            PasswordHash = passwordHash;
            HashSalt = hashSalt;
            Gender = gender;
            UserType = userType;
            Address = address;
            ProfilePictureUrl = profilePictureUrl;
        }

        
    }
}