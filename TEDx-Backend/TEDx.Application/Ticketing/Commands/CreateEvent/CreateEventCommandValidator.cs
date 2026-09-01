using TEDx.Application.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TEDx.Application.Ticketing.Commands.CreateEvent
{
    public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandValidator()
        {
            RuleFor(x => x.TitleEn).NotEmpty().WithMessage("English title is required.")
                .MaximumLength(200).WithMessage("English title must not exceed 200 characters.");

            RuleFor(x => x.TitleAr).NotEmpty().WithMessage("Arabic title is required.")
                .MaximumLength(200).WithMessage("Arabic title must not exceed 200 characters.");

            RuleFor(x => x.DescriptionAr).NotEmpty().WithMessage("Arabic description is required.");

            RuleFor(x => x.DescriptionEn).NotEmpty().WithMessage("English description is required.");

            RuleFor(x => x.Location).NotEmpty().WithMessage("Location is required.")
                .MaximumLength(300).WithMessage("Location must not exceed 300 characters.");

            RuleFor(x => x.ImageUrl).MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.");

            RuleFor(x => x.StartsAtUtc).LessThan(x => x.EndsAtUtc).WithMessage("Start date must be before end date.");

            RuleFor(x => x.Capacity).GreaterThan(0).WithMessage("Capacity must be greater than 0.").WithErrorCode("INVALID_CAPACITY");

            RuleFor(x => x.TicketPrice)
                .NotNull();

            RuleFor(x => x.TicketPrice.Amount)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode("INVALID_TICKET_PRICE");

            RuleFor(x => x.TicketPrice.Currency)
                .NotEmpty()
                .Equal(CurrencyCodes.Egp);
        }
    }
}
