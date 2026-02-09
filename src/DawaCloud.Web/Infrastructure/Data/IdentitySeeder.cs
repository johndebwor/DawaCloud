using DawaCloud.Web.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DawaCloud.Web.Infrastructure.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        // Seed Roles
        var roles = new[]
        {
            "SuperAdmin",
            "InventoryManager",
            "Pharmacist",
            "Cashier",
            "Accountant",
            "Auditor",
            "Supplier"
        };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} role"
                };
                await roleManager.CreateAsync(role);
            }
        }

        // Seed Admin User
        var adminEmail = "admin@dawaflow.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123456");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
            }
        }

        // Seed Demo Pharmacist
        var pharmacistEmail = "pharmacist@dawaflow.com";
        var pharmacistUser = await userManager.FindByEmailAsync(pharmacistEmail);

        if (pharmacistUser == null)
        {
            pharmacistUser = new ApplicationUser
            {
                UserName = pharmacistEmail,
                Email = pharmacistEmail,
                FirstName = "John",
                LastName = "Deng",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(pharmacistUser, "Demo@123456");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(pharmacistUser, "Pharmacist");
            }
        }

        // Seed Demo Cashier
        var cashierEmail = "cashier@dawaflow.com";
        var cashierUser = await userManager.FindByEmailAsync(cashierEmail);

        if (cashierUser == null)
        {
            cashierUser = new ApplicationUser
            {
                UserName = cashierEmail,
                Email = cashierEmail,
                FirstName = "Jane",
                LastName = "Ayaa",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(cashierUser, "Demo@123456");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(cashierUser, "Cashier");
            }
        }
    }
}
