using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using TEDx.Domain.Ticketing.Enums;  

namespace TEDx.Application.Ticketing.Command.ChangeEventStatus
{
    public class ChangeEventStatusCommandValidator : AbstractValidator<ChangeEventStatusCommand>
    {
        public ChangeEventStatusCommandValidator()
        {
            RuleFor(x => x.TargetStatus)
                .Must(status =>
                    status == EventStatus.Draft ||
                    status == EventStatus.Published ||
                    status == EventStatus.Archived) 
                .WithErrorCode("VALIDATION_ERROR");
        }
    }
}
