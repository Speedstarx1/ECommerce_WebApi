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
            string passwordHash,
            string hashSalt)
            : base(firstName, lastName, email, passwordHash, hashSalt)
        {
            RefNumber = GenerateAdminNumber();

            UserType = UserType.Admin;
        }

        // EF Core parameterless constructor
        protected Admin() : base("", "", "", "", "") { }

        private static string GenerateAdminNumber()
        {
            return $"ADM-{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 7).ToUpper()}";
        }
    }

}


