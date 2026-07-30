namespace TEDx.Infrastructure.Persistence.Seeding;

internal sealed record AdminSeederOptions(string Email, string Password)
{
    public const string EmailKey = "ADMIN_EMAIL";
    public const string PasswordEnvVar = "ADMIN_PASSWORD";
}
