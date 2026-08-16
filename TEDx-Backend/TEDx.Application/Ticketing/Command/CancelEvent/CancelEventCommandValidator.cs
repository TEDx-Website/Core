using FluentValidation;

namespace TEDx.Application.Ticketing.Command.CancelEvent
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
