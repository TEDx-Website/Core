using System;
using System.Linq.Expressions;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Queries
{
    public static class RemainingSeatsProjection
    {
        // this can be used in EF Core queries to calculate the remaining seats for an event , not a C# logic, so we need to use Expression<Func<Event, int>> instead of Func<Event, int>
        public static Expression<Func<Event, int>> For(DateTime now)
        {
            return @event =>
                @event.Capacity
                -
                @event.Orders!
                    .Where(order =>
                        order.Status == OrderStatus.Paid
                        ||
                        (
                            order.Status == OrderStatus.PendingPayment
                            && order.HoldExpiresAtUtc > now
                        ))
                    .Sum(order => order.Quantity);
        }
    }
}
