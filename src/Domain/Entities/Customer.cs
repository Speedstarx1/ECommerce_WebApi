using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Customer : User
    {
        public string RefNumber { get; set; } = default!;
            

        public Customer(
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            string hashSalt)
            : base(firstName, lastName, email, passwordHash, hashSalt)
        {
            RefNumber = GenerateCustomerNumber();
            UserType = UserType.Customer;
            
        }

        // EF Core parameterless constructor
        protected Customer() : base("", "", "", "", "", "", null, "") { }
        private static string GenerateCustomerNumber()
        {
            return $"CST-{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 7).ToUpper()}";
        }
    }
}
    

