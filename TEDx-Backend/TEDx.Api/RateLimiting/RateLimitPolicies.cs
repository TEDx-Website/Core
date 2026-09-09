namespace TEDx.Api.RateLimiting;

public static class RateLimitPolicies
{
    public const string Auth = "auth";
    public const string AuthMail = "auth-mail";
    public const string Upload = "upload";

    public const string Contact = "contact";

    // Every endpoint reachable without a token. POST /contact sits in both groups but
    // takes the stricter Contact one: EnableRateLimiting replaces, it does not stack.
    public const string Anonymous = "anonymous";

    public static readonly IReadOnlyList<string> All = [Auth, AuthMail, Upload, Contact, Anonymous];
}
