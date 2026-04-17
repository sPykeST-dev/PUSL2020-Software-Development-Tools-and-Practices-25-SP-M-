using BlindMatch.Core.Common;
using BlindMatch.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlindMatch.Infrastructure.Identity;

public static class UserSeeder
{
    public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
    {
        const string email = "admin@blindmatch.ac.lk";

        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName       = email,
            Email          = email,
            FirstName      = "System",
            LastName       = "Administrator",
            IsActive       = true,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(admin, "Admin@BlindMatch1");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, Roles.SystemAdministrator);
    }
}
