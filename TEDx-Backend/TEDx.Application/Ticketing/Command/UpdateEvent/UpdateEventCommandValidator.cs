using FluentValidation;

namespace TEDx.Application.Ticketing.Command.UpdateEvent
{
    public sealed class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("Event ID is required.");

            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage("English title is required.")
                .MaximumLength(200).WithMessage("English title must not exceed 200 characters.");

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage("Arabic title is required.")
                .MaximumLength(200).WithMessage("Arabic title must not exceed 200 characters.");

            RuleFor(x => x.DescriptionEn)
                .NotEmpty().WithMessage("English description is required.");

            RuleFor(x => x.DescriptionAr)
                .NotEmpty().WithMessage("Arabic description is required.");

            RuleFor(x => x.Venue)
                .NotEmpty().WithMessage("Venue is required.")
                .MaximumLength(300).WithMessage("Venue must not exceed 300 characters.");

            RuleFor(x => x.StartsAtUtc)
                .LessThan(x => x.EndsAtUtc).WithMessage("Start date must be before end date.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0.")
                .WithErrorCode("INVALID_CAPACITY");

            RuleFor(x => x.TicketPrice)
                .NotNull().WithMessage("Ticket price is required.");

            When(x => x.TicketPrice is not null, () =>
            {
                RuleFor(x => x.TicketPrice.Amount)
                    .GreaterThanOrEqualTo(0).WithMessage("Ticket price amount must be 0 or greater.")
                    .WithErrorCode("INVALID_TICKET_PRICE");

                RuleFor(x => x.TicketPrice.Currency)
                    .NotEmpty().WithMessage("Currency is required.")
                    .Equal("EGP").WithMessage("Only EGP currency is supported.");
            });

            RuleFor(x => x.MaxIndividualQtyPerOrder)
                .GreaterThan(0).When(x => x.MaxIndividualQtyPerOrder.HasValue)
                .WithMessage("Max individual quantity per order must be greater than 0.")
                .WithErrorCode("INVALID_MAX_QTY");

            RuleFor(x => x.RowVersion)
                .NotNull().WithMessage("RowVersion is required for concurrency control.")
                .NotEmpty().WithMessage("RowVersion must not be empty.");
        }
    }
}
