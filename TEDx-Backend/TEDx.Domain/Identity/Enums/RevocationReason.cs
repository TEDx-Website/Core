namespace TEDx.Domain.Identity.Enums
{
    public enum RevocationReason
    {
        Rotated = 0,
        Reuse = 1,
        Logout = 2,
        Expired = 3,
        PasswordReset = 4,
        PasswordChange = 5
    }
}
