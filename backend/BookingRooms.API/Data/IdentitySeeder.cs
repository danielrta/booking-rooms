using BookingRooms.API.Features.Auth;
using Microsoft.AspNetCore.Identity;

namespace BookingRooms.API.Data;

public static class IdentitySeeder
{
    public static async Task SeedDefaultUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        
        const string adminRole = "Admin";
        const string userRole = "User";
        
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }
        
        if (!await roleManager.RoleExistsAsync(userRole))
        {
            await roleManager.CreateAsync(new IdentityRole(userRole));
        }
        
        const string defaultUserName = "admin";
        const string defaultPassword = "Admin@123";
        
        var adminUser = await userManager.FindByNameAsync(defaultUserName);
       
        if (adminUser is null)
        {
            var newUser = new ApplicationUser
            {
                UserName = defaultUserName,
                Email = "admin@test.com",
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(newUser, defaultPassword);
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, adminRole);
            }
        }
    }
}