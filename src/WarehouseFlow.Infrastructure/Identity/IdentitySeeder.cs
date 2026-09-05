using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Enum;

namespace WarehouseFlow.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in Enum.GetNames<UserRole>())
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Could not create role '{role}'.");
                }
            }
        }
    }

    public static async Task SeedSuperAdminAsync(
        IServiceProvider services,
        IConfiguration configuration
    )
    {
        const string email = "hoxtygen@live.com";
        const string roleName = nameof(UserRole.Super_Admin);

        var password =
            configuration["IdentitySeed:SuperAdminPassword"]
            ?? throw new InvalidOperationException(
                "IdentitySeed:SuperAdminPassword must be set via user-secrets or env vars."
            );

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "IdentitySeed:SuperAdminPassword must be configured before seeding the Super Admin."
            );
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = "System",
                LastName = "Administrator",
                PhoneNumber = "+2348012345678",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            var createResult = await userManager.CreateAsync(user, password);
            EnsureSucceeded(createResult, "Could not create the Super Admin account.");
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            var roleResult = await userManager.AddToRoleAsync(user, roleName);
            EnsureSucceeded(roleResult, "Could not assign the Super Admin role.");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"{message} {errors}");
        }
    }
}
