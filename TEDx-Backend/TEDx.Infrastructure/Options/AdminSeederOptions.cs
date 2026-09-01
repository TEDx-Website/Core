namespace TEDx.Infrastructure.Persistence.Seeding;

internal sealed class AdminSeederOptions
{
    public const string EmailKey = "ADMIN_EMAIL";
    public const string PasswordEnvVar = "ADMIN_PASSWORD";

    // Parameterless ctor needed for IConfiguration.Get<List<AdminSeederOptions>>()
    public AdminSeederOptions() { }

    public AdminSeederOptions(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
