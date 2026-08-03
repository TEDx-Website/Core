using FluentValidation;

namespace TEDx.Application.Identity.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private const int MinPasswordLength = 8;

    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must be at most 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must be at most 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Enter a valid email address.");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(MinPasswordLength)
                .WithMessage($"The password must be at least {MinPasswordLength} characters.")
            .Must(p => p.Any(char.IsUpper)).WithMessage("The password must contain at least one uppercase letter.")
            .Must(p => p.Any(char.IsLower)).WithMessage("The password must contain at least one lowercase letter.")
            .Must(p => p.Any(char.IsDigit)).WithMessage("The password must contain at least one number.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}
