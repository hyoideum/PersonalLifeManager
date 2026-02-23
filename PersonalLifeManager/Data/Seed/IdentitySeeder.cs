using Microsoft.AspNetCore.Identity;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Data.Seed;

public class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new AppRole { Name = "Admin" });

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new AppRole { Name = "User" });
        
        var adminEmail = "admin@plm.com";

        var admin = await userManager.FindByEmailAsync(adminEmail);
        
        if (admin == null)
        {
            var user = new AppUser
            {
                UserName = "superadmin",
                FirstName = "Super",
                LastName = "Admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, "Admin123!");

            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}