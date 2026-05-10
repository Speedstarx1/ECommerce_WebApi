using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeders
{
    public static class CustomerSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                // Check if data already exists
                if (await context.Customers.AnyAsync())
                {
                    logger.LogInformation("Database already seeded. Skipping seeding.");
                    return;
                }

                logger.LogInformation("Seeding database with initial Customer data...");

                // Generate password hash for "Customer@123"
                var (hash, salt) = UserHelper.GeneratePasswordHash("Customer@123");
                var customers = new List<Customer>
                {
                    new Customer(
                        firstName: "Samuel",
                        lastName: "Sho",
                        email: "samuelsho@gmail.com",
                        passwordHash: hash,
                        hashSalt: salt
                    )

                };

                await context.Customers.AddRangeAsync(customers);
                await context.SaveChangesAsync();

                logger.LogInformation("Successfully seeded {Count} customers.", customers.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}