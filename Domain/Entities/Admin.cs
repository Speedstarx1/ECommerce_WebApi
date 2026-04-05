using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Admin : User
    {
        public string RefNumber { get; set; } = default!;


        public Admin(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string passwordHash,
            string hashSalt,
            Gender gender,
            UserType userType,
            string address,
            string createdBy,
            DateTime createdDate,
            string? profilePictureUrl = null)
            : base(firstName, lastName, email, phoneNumber, passwordHash, hashSalt, gender, UserType.Customer, address, profilePictureUrl)
        {
            RefNumber = GenerateAdminNumber();

            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }

        // EF Core parameterless constructor
        protected Admin() : base("", "", "", "", "", "", Gender.Male, UserType.Customer, "", "") { }

        private string GenerateAdminNumber()
        {
            return $"ADM-{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 7).ToUpper()}";
        }
    }

}


