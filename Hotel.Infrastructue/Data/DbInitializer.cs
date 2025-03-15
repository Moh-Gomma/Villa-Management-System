using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastructue.Data
{
    public class DbInitializer
    {
        public static async Task SeedDefaultAdmin(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(SD.Role_Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
            }

            if (!await roleManager.RoleExistsAsync(SD.Role_Customer))
            {
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
            }

            var adminEmail = configuration["DefaultAdmin:Email"];
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var defaultAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    EmailConfirmed = true,
                    FirstName = configuration["DefaultAdmin:FirstName"],
                    LastName = configuration["DefaultAdmin:LastName"],
                    PhoneNumber = configuration["DefaultAdmin:PhoneNumber"],
                    CreationTime = DateTime.Now
                };

                var password = configuration["DefaultAdmin:Password"];
                var result = await userManager.CreateAsync(defaultAdmin, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultAdmin, SD.Role_Admin);
                    Console.WriteLine("Default admin user created successfully.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating default admin user: {error.Description}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Default admin user already exists.");
            }
        }
    }
}
