using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Identity.Enums;

namespace TEDx.Infrastructure.Persistence.Seeding;

public sealed class AdminSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminSeeder> _logger;

    public AdminSeeder(
        UserManager<User> userManager,
        IConfiguration configuration,
        ILogger<AdminSeeder> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var options = ResolveOptions();

        await EnsureAdminUserAsync(options);
    }

    private AdminSeederOptions ResolveOptions()
    {
        var email = _configuration[AdminSeederOptions.EmailKey]
                    ?? _configuration["Admin:Email"];
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException(
                $"Admin seeding requires configuration key '{AdminSeederOptions.EmailKey}' or 'Admin:Email'. " +
                "Set it in configuration before startup.");
        }

        var password = Environment.GetEnvironmentVariable(AdminSeederOptions.PasswordEnvVar);
        if (string.IsNullOrWhiteSpace(password))
        {
            password = _configuration[AdminSeederOptions.PasswordEnvVar]
                       ?? _configuration["Admin:Password"];
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Admin seeding requires environment variable '{AdminSeederOptions.PasswordEnvVar}' " +
                "or configuration key 'ADMIN_PASSWORD' / 'Admin:Password'. Set it before startup.");
        }

        return new AdminSeederOptions(email.Trim(), password);
    }


    private async Task EnsureAdminUserAsync(AdminSeederOptions options)
    {
        var existing = await _userManager.FindByEmailAsync(options.Email);
        if (existing is not null)
        {
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

        // Admin is the GlobalRole column only — never an Identity role (D:Q36, D:Q46).
        _logger.LogInformation("Seeded admin account {Email}.", options.Email);
    }

    private static string DescribeErrors(IdentityResult result) =>
        string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
}
