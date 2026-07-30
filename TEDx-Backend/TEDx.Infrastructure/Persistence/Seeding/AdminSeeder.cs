using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Identity.Enums;

namespace TEDx.Infrastructure.Persistence.Seeding;

public sealed class AdminSeeder
{
    public const string AdminRoleName = "Admin";

    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminSeeder> _logger;

    public AdminSeeder(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration,
        ILogger<AdminSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var options = ResolveOptions();

        await EnsureAdminRoleAsync();
        await EnsureAdminUserAsync(options);
    }

    private AdminSeederOptions ResolveOptions()
    {
        var email = _configuration[AdminSeederOptions.EmailKey];
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException(
                $"Admin seeding requires configuration key '{AdminSeederOptions.EmailKey}'. " +
                "Set it in configuration before startup.");
        }

        var password = Environment.GetEnvironmentVariable(AdminSeederOptions.PasswordEnvVar);
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Admin seeding requires environment variable '{AdminSeederOptions.PasswordEnvVar}'. " +
                "Set it before startup; the admin password must never be hardcoded.");
        }

        return new AdminSeederOptions(email.Trim(), password);
    }

    private async Task EnsureAdminRoleAsync()
    {
        if (await _roleManager.RoleExistsAsync(AdminRoleName))
        {
            return;
        }

        var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(AdminRoleName));
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create '{AdminRoleName}' role: {DescribeErrors(result)}");
        }

        _logger.LogInformation("Created identity role {RoleName}.", AdminRoleName);
    }

    private async Task EnsureAdminUserAsync(AdminSeederOptions options)
    {
        var existing = await _userManager.FindByEmailAsync(options.Email);
        if (existing is not null)
        {
            await EnsureInAdminRoleAsync(existing);
            _logger.LogInformation("Admin account already present; skipping creation.");
            return;
        }

        var admin = new User
        {
            UserName = options.Email,
            Email = options.Email,
            EmailConfirmed = true,
            Role = GlobalRole.Admin,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = "system:seed",
        };

        var created = await _userManager.CreateAsync(admin, options.Password);
        if (!created.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create admin account: {DescribeErrors(created)}");
        }

        await EnsureInAdminRoleAsync(admin);
        _logger.LogInformation("Seeded admin account {Email}.", options.Email);
    }

    private async Task EnsureInAdminRoleAsync(User user)
    {
        if (await _userManager.IsInRoleAsync(user, AdminRoleName))
        {
            return;
        }

        var result = await _userManager.AddToRoleAsync(user, AdminRoleName);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to add admin to '{AdminRoleName}' role: {DescribeErrors(result)}");
        }
    }

    private static string DescribeErrors(IdentityResult result) =>
        string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
}
