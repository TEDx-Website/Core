using FluentValidation;

namespace TEDx.Application.Ticketing.Command.UpdateEvent
{
    public sealed class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        private const int TitleMaxLength = 200;
        private const int DescriptionMaxLength = 3000;
        private const int LocationMaxLength = 300;

        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("Event ID is required.");

            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage("English title is required.")
                .MaximumLength(TitleMaxLength).WithMessage($"English title must not exceed {TitleMaxLength} characters.");

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage("Arabic title is required.")
                .MaximumLength(TitleMaxLength).WithMessage($"Arabic title must not exceed {TitleMaxLength} characters.");

            RuleFor(x => x.DescriptionEn)
                .NotEmpty().WithMessage("English description is required.")
                .MaximumLength(DescriptionMaxLength).WithMessage($"English description must not exceed {DescriptionMaxLength} characters.");

            RuleFor(x => x.DescriptionAr)
                .NotEmpty().WithMessage("Arabic description is required.")
                .MaximumLength(DescriptionMaxLength).WithMessage($"Arabic description must not exceed {DescriptionMaxLength} characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(LocationMaxLength).WithMessage($"Location must not exceed {LocationMaxLength} characters.");

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
                .NotEmpty().WithMessage("RowVersion is required for concurrency control.");
        }
    }
}
