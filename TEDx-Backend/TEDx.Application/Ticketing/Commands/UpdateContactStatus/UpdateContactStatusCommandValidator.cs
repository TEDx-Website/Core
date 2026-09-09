using FluentValidation;
using TEDx.Domain.Communication.Enums;
namespace TEDx.Application.Ticketing.Commands.UpdateContactStatus
{
    public sealed class UpdateContactStatusCommandValidator
    : AbstractValidator<UpdateContactStatusCommand>
    {
        public UpdateContactStatusCommandValidator()
        {
            RuleFor(x => x.Status)
                .Must(status =>
                    status == ContactStatus.Read ||
                    status == ContactStatus.Archived)
                .WithMessage("Status must be Read or Archived.")
                .WithErrorCode("VALIDATION_ERROR");
        }
    }
}
