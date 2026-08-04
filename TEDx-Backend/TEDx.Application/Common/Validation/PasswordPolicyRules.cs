using FluentValidation;

namespace TEDx.Application.Common.Validation;

public static class PasswordPolicyRules
{
    public const int MinLength = 8;

    public static IRuleBuilderOptions<T, string> PasswordPolicy<T>(
        this IRuleBuilderInitial<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(MinLength)
                .WithMessage($"The password must be at least {MinLength} characters.")
            .Must(p => p.Any(char.IsUpper))
                .WithMessage("The password must contain at least one uppercase letter.")
            .Must(p => p.Any(char.IsLower))
                .WithMessage("The password must contain at least one lowercase letter.")
            .Must(p => p.Any(char.IsDigit))
                .WithMessage("The password must contain at least one number.");
    }
}
