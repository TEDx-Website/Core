using FluentValidation;
using TEDx.Application.Common.Pagination;

namespace TEDx.Application.Ticketing.Queries.GetEventPromoCodes;

public sealed class GetEventPromoCodesQueryValidator
    : AbstractValidator<GetEventPromoCodesQuery>
{
    public GetEventPromoCodesQueryValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty();

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(PagedDefaults.MinPage);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(PagedDefaults.MinPageSize, PagedDefaults.MaxPageSize);
    }
}
