using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Reflection;

namespace Infrastructure.Persistence.Seeders
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                if (!await context.Admins.AnyAsync())
                {
                    var (hash, salt) = UserHelper.GeneratePasswordHash("Admin@123");
                    var administrator = new Admin(
                        firstName: "System",
                        lastName: "Administrator",
                        email: "admin@studentreg.com",
                        phoneNumber: "09088776654",
                        passwordHash: hash,
                        hashSalt: salt,
                        gender: Gender.Male,
                        userType: UserType.Admin,
                        address: "System",
                        createdBy: "System",
                        createdDate: DateTime.UtcNow
                    );



                    context.Admins.Add(administrator);

                    await context.SaveChangesAsync();
                    logger.LogInformation("Administrator seeded successfully. Email: admin@studentreg.com, Password: Admin@123");
                }
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding Administrator");
                throw;
            }
        }
    }
}