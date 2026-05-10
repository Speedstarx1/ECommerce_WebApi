using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using System.Reflection.Metadata.Ecma335;

namespace Application.Helpers
{
    public static class UserHelper
    {
        public static (string hash, string salt) GeneratePasswordHash(string password)
        {
            var salt = Guid.NewGuid().ToString("N")[..16];
            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes($"{password}{salt}")));
            return (hash, salt);
        }

        

        

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes($"{password}{storedSalt}")));
            return hash == storedHash;
        }
    }
}

