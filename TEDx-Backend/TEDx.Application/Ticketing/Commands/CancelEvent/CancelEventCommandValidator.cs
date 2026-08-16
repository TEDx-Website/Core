using FluentValidation;

namespace TEDx.Application.Ticketing.Commands.CancelEvent
{
    public sealed class CancelEventCommandValidator : AbstractValidator<CancelEventCommand>
    {
        public CancelEventCommandValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage("Event ID is required.");
        }
    }
}
