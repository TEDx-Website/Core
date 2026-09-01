using FluentValidation;

namespace TEDx.Application.Ticketing.Queries.GetPublicEvents;

public sealed class GetPublicEventsQueryValidator
    : AbstractValidator<GetPublicEventsQuery>
{


    public GetPublicEventsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.When)
            .Must(when =>
                string.IsNullOrWhiteSpace(when)
                || string.Equals(
                    when.Trim(),
                    "upcoming",
                    StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    when.Trim(),
                    "past",
                    StringComparison.OrdinalIgnoreCase))
            .WithMessage("When must be one of: upcoming, past.");
    }
}
