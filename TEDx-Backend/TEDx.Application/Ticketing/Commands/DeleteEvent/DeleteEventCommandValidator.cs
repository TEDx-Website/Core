using FluentValidation;

namespace TEDx.Application.Ticketing.Commands.DeleteEvent;

public sealed class DeleteEventCommandValidator : AbstractValidator<DeleteEventCommand>
{
    public DeleteEventCommandValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty();
    }
}
