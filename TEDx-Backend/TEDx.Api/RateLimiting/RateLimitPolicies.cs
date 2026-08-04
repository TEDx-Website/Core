namespace TEDx.Api.RateLimiting;

public static class RateLimitPolicies
{
    public const string Auth = "auth";
    public const string AuthMail = "auth-mail";

    public static readonly IReadOnlyList<string> All = [Auth, AuthMail];
}
