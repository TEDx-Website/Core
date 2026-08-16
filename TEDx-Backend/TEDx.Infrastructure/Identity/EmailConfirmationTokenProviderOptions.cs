using Microsoft.AspNetCore.Identity;

namespace TEDx.Infrastructure.Identity;

public sealed class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public EmailConfirmationTokenProviderOptions()
    {
        // Distinct data-protection purpose string — not the DI lookup key.
        Name = "EmailConfirmationTokenProvider";
        TokenLifespan = TimeSpan.FromHours(24);
    }
}
