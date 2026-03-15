using LastMile.TMS.Domain.Entities;
using LastMile.TMS.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LastMile.TMS.Persistence;

/// <summary>
/// Hosted service that seeds default roles and the admin user on first startup.
/// </summary>
public sealed class DbSeeder(
    IServiceScopeFactory scopeFactory,
    ILogger<DbSeeder> logger,
    IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await SeedRolesAsync(roleManager, cancellationToken);
        await SeedAdminUserAsync(userManager, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // ── Roles ──────────────────────────────────────────────────────────────────

    private async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, CancellationToken ct)
    {
        var roles = Enum.GetValues<PredefinedRole>();

        foreach (var role in roles)
        {
            var name = role.ToString();
            if (await roleManager.RoleExistsAsync(name))
                continue;

            var result = await roleManager.CreateAsync(new ApplicationRole
            {
                Name = name,
                IsDefault = role == PredefinedRole.Driver
            });

            if (result.Succeeded)
                logger.LogInformation("Seeded role: {Role}", name);
            else
                logger.LogError("Failed to seed role {Role}: {Errors}", name, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // ── Admin User ─────────────────────────────────────────────────────────────

    private async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, CancellationToken ct)
    {
        var email = configuration["AdminCredentials:Email"] ?? "admin@lastmile.com";
        var password = configuration["AdminCredentials:Password"] ?? "Admin@12345";

        // Skip if an admin already exists
        var existingAdmins = await userManager.GetUsersInRoleAsync(PredefinedRole.Admin.ToString());
        if (existingAdmins.Any())
        {
            logger.LogDebug("Admin user already exists — skipping seed");
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "System",
            LastName = "Admin",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = "Seeder"
        };

        var createResult = await userManager.CreateAsync(admin, password);
        if (!createResult.Succeeded)
        {
            logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return;
        }

        var roleResult = await userManager.AddToRoleAsync(admin, PredefinedRole.Admin.ToString());
        if (roleResult.Succeeded)
            logger.LogInformation("Seeded admin user: {Email}", email);
        else
            logger.LogError("Failed to assign Admin role to seeded user: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
    }
}
