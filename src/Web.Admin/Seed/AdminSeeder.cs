using Domain.Entities;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;

namespace Web.Admin.Seed;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var email = "admin@edutrack.com";
        var password = "Admin123*";

        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(admin, password);
    }
}