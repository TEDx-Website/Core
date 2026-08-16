using FluentValidation;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Command.ChangeEventStatus
{
    public sealed class ChangeEventStatusCommandValidator : AbstractValidator<ChangeEventStatusCommand>
    {
        public ChangeEventStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.TargetStatus)
                .Must(BeAdminSelectable)
                .WithErrorCode("VALIDATION_ERROR")
                .WithMessage(
                    "Only Draft, Published or Archived can be set here. " +
                    "To cancel an event use POST /api/v1/admin/events/{id}/cancel.");
        }

        private static bool BeAdminSelectable(EventStatus status)
            => status is EventStatus.Draft or EventStatus.Published or EventStatus.Archived;
    }
}
