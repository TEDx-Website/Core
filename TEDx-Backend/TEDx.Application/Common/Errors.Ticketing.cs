using TEDx.Domain.Common;

namespace TEDx.Application.Common;

public static partial class Errors
{
    public static readonly Error EventNotPublished =
        new("EVENT_NOT_PUBLISHED", "The event is not published.", ErrorType.Business);

    public static readonly Error EventHasOrders =
        new("EVENT_HAS_ORDERS", "The event has orders and cannot be modified this way.", ErrorType.Conflict);

    public static readonly Error HasOrdersCannotUnpublish =
        new("HAS_ORDERS_CANNOT_UNPUBLISH", "The event has orders and cannot be unpublished.", ErrorType.Conflict);

    public static readonly Error CapacityBelowSold =
        new("CAPACITY_BELOW_SOLD", "Capacity cannot be set below the number already sold.", ErrorType.Conflict);

    public static readonly Error InvalidCapacity =
        new("INVALID_CAPACITY", "The capacity value is invalid.", ErrorType.Validation);

    public static readonly Error InvalidTicketPrice =
        new("INVALID_TICKET_PRICE", "The ticket price is invalid.", ErrorType.Validation);

    public static readonly Error NoPackages =
        new("NO_PACKAGES", "The event has no packages to publish.", ErrorType.Business);

    public static readonly Error PackageReferencedByOrders =
        new("PACKAGE_REFERENCED_BY_ORDERS", "The package is referenced by orders and cannot be deleted.", ErrorType.Conflict);

    public static readonly Error SeatsUnavailable =
        new("SEATS_UNAVAILABLE", "Not enough seats are available.", ErrorType.Conflict);

    public static readonly Error PriceChanged =
        new("PRICE_CHANGED", "The price has changed since it was quoted.", ErrorType.Conflict);

    public static readonly Error QuantityExceedsMax =
        new("QUANTITY_EXCEEDS_MAX", "The requested quantity exceeds the maximum allowed.", ErrorType.Business);

    public static readonly Error ActiveOrderExists =
        new("ACTIVE_ORDER_EXISTS", "An active order already exists for this event.", ErrorType.Conflict);

    public static readonly Error HoldExpired =
        new("HOLD_EXPIRED", "The seat hold has expired.", ErrorType.Conflict);

    public static readonly Error OrderNotCancellable =
        new("ORDER_NOT_CANCELLABLE", "The order cannot be cancelled in its current state.", ErrorType.Conflict);

    public static readonly Error OrderNotPayable =
        new("ORDER_NOT_PAYABLE", "The order cannot be paid in its current state.", ErrorType.Conflict);

    public static readonly Error OrderIsFree =
        new("ORDER_IS_FREE", "The order is free and does not require payment.", ErrorType.Conflict);

    public static readonly Error OrderNotFree =
        new("ORDER_NOT_FREE", "The order is not free.", ErrorType.Conflict);

    public static readonly Error OrderNotVoidable =
        new("ORDER_NOT_VOIDABLE", "The order cannot be voided in its current state.", ErrorType.Conflict);

    public static readonly Error InvalidSignature =
        new("INVALID_SIGNATURE", "The webhook signature is invalid.", ErrorType.Unauthorized);

    // Check-in (API §11).
    public static readonly Error TicketAlreadyCheckedIn =
        new("TICKET_ALREADY_CHECKED_IN", "The ticket has already been checked in.", ErrorType.Conflict);

    public static readonly Error WrongEvent =
        new("WRONG_EVENT", "The ticket belongs to a different event.", ErrorType.Conflict);

    public static readonly Error TicketVoided =
        new("TICKET_VOIDED", "The ticket has been voided.", ErrorType.Conflict);

    public static readonly Error TicketInvalid =
        new("TICKET_INVALID", "The ticket reference or secret is invalid.", ErrorType.NotFound);

    // Promo codes (API §7).
    public static readonly Error PromoCodeTaken =
        new("PROMO_CODE_TAKEN", "This promo code already exists.", ErrorType.Conflict);

    public static readonly Error PromoInactive =
        new("PROMO_INACTIVE", "The promo code is inactive.", ErrorType.Business);

    public static readonly Error PromoNotYetValid =
        new("PROMO_NOT_YET_VALID", "The promo code is not yet valid.", ErrorType.Business);

    public static readonly Error PromoExpired =
        new("PROMO_EXPIRED", "The promo code has expired.", ErrorType.Business);

    public static readonly Error PromoCapReached =
        new("PROMO_CAP_REACHED", "The promo code redemption cap has been reached.", ErrorType.Conflict);

    public static readonly Error PromoUserLimit =
        new("PROMO_USER_LIMIT", "You have already used this promo code the maximum number of times.", ErrorType.Business);

    public static readonly Error PromoWrongEvent =
        new("PROMO_WRONG_EVENT", "The promo code is not valid for this event.", ErrorType.Business);
}
