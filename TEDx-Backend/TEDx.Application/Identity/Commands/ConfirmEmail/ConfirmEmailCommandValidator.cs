using FluentValidation;

namespace TEDx.Application.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Enter a valid email address.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Confirmation token is required.");
    }
}
