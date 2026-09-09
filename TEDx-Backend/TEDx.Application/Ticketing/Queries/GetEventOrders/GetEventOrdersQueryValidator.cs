using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TEDx.Application.Ticketing.Queries.GetEventOrders
{
    public sealed class GetEventOrdersQueryValidator
     : AbstractValidator<GetEventOrdersQuery>
    {
        public GetEventOrdersQueryValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty();

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100);
        }
    }
}
