namespace TEDx.Api.RateLimiting;

public static class RateLimitPolicies
{
    public const string Auth = "auth";
    public const string AuthMail = "auth-mail";
    public const string Upload = "upload";

    public const string Contact = "contact";

    public static readonly IReadOnlyList<string> All = [Auth, AuthMail, Upload, Contact];
}
