using System.Security.Cryptography;
using System.Text;

namespace TEDx.Infrastructure.Identity;

internal static class RefreshTokenGenerator
{
    private const int TokenBytes = 32;

    public static string CreateRaw() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenBytes));
    
    public static string Hash(string raw) =>
        Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
}
