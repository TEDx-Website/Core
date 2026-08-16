using TEDx.Domain.Common;

namespace TEDx.Application.Common.Errors;

public static partial class Errors_Ticketing
{
    public static readonly Error EventNotPublished =
    Error.Business(
        "EVENT_NOT_PUBLISHED",
        "The event is not published.");

    public static readonly Error EventHasOrders =
        Error.Conflict(
            "EVENT_HAS_ORDERS",
            "The event has orders and cannot be modified this way.");

    public static readonly Error HasOrdersCannotUnpublish =
        Error.Conflict(
            "HAS_ORDERS_CANNOT_UNPUBLISH",
            "The event has orders and cannot be unpublished. Cancel the event instead.");

    public static readonly Error CapacityBelowSold =
        Error.Conflict(
            "CAPACITY_BELOW_SOLD",
            "Capacity cannot be set below the number already sold.");

    public static readonly Error InvalidCapacity =
        Error.Validation(
            "INVALID_CAPACITY",
            "The capacity value is invalid.");

    public static readonly Error InvalidTicketPrice =
        Error.Validation(
            "INVALID_TICKET_PRICE",
            "The ticket price is invalid.");

    public static readonly Error PackageReferencedByOrders =
        Error.Conflict(
            "PACKAGE_REFERENCED_BY_ORDERS",
            "The package is referenced by orders and cannot be deleted.");

    public static readonly Error SeatsUnavailable =
        Error.Conflict(
            "SEATS_UNAVAILABLE",
            "Not enough seats are available.");

    public static readonly Error PriceChanged =
        Error.Conflict(
            "PRICE_CHANGED",
            "The price has changed since it was quoted.");

    public static readonly Error QuantityExceedsMax =
        Error.Business(
            "QUANTITY_EXCEEDS_MAX",
            "The requested quantity exceeds the maximum allowed.");

    public static readonly Error ActiveOrderExists =
        Error.Conflict(
            "ACTIVE_ORDER_EXISTS",
            "An active order already exists for this event.");

    public static readonly Error HoldExpired =
        Error.Conflict(
            "HOLD_EXPIRED",
            "The seat hold has expired.");

    public static readonly Error OrderNotCancellable =
        Error.Conflict(
            "ORDER_NOT_CANCELLABLE",
            "The order cannot be cancelled in its current state.");

    public static readonly Error OrderNotPayable =
        Error.Conflict(
            "ORDER_NOT_PAYABLE",
            "The order cannot be paid in its current state.");

    public static readonly Error OrderIsFree =
        Error.Conflict(
            "ORDER_IS_FREE",
            "The order is free and does not require payment.");

    public static readonly Error OrderNotFree =
        Error.Conflict(
            "ORDER_NOT_FREE",
            "The order is not free.");

    public static readonly Error OrderNotVoidable =
        Error.Conflict(
            "ORDER_NOT_VOIDABLE",
            "The order cannot be voided in its current state.");

    public static readonly Error InvalidSignature =
        Error.Unauthorized(
            "INVALID_SIGNATURE",
            "The webhook signature is invalid.");

    public static readonly Error TicketAlreadyCheckedIn =
        Error.Conflict(
            "TICKET_ALREADY_CHECKED_IN",
            "The ticket has already been checked in.");

    public static readonly Error WrongEvent =
        Error.Conflict(
            "WRONG_EVENT",
            "The ticket belongs to a different event.");

    public static readonly Error TicketVoided =
        Error.Conflict(
            "TICKET_VOIDED",
            "The ticket has been voided.");

    public static readonly Error TicketInvalid =
        Error.NotFound(
            "TICKET_INVALID",
            "The ticket reference or secret is invalid.");

    public static readonly Error PromoCodeTaken =
        Error.Conflict(
            "PROMO_CODE_TAKEN",
            "This promo code already exists.");

    public static readonly Error PromoInactive =
        Error.Business(
            "PROMO_INACTIVE",
            "The promo code is inactive.");

    public static readonly Error PromoNotYetValid =
        Error.Business(
            "PROMO_NOT_YET_VALID",
            "The promo code is not yet valid.");

    public static readonly Error PromoExpired =
        Error.Business(
            "PROMO_EXPIRED",
            "The promo code has expired.");

    public static readonly Error PromoCapReached =
        Error.Conflict(
            "PROMO_CAP_REACHED",
            "The promo code redemption cap has been reached.");

    public static readonly Error PromoUserLimit =
        Error.Business(
            "PROMO_USER_LIMIT",
            "You have already used this promo code the maximum number of times.");

    public static readonly Error PromoWrongEvent =
        Error.Business(
            "PROMO_WRONG_EVENT",
            "The promo code is not valid for this event.");

}
