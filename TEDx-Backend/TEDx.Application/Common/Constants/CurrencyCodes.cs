namespace TEDx.Application.Common.Constants;

/// <summary>
/// ISO-4217 currency codes the platform accepts. EGP is the only one today
/// (Data Model §2.1 pins <c>Currency nvarchar(3) DEFAULT 'EGP'</c>).
/// </summary>
public static class CurrencyCodes
{
    public const string Egp = "EGP";
}
